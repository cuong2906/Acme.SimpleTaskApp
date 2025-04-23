using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Acme.SimpleTaskApp.Cart.Dto;
using Acme.SimpleTaskApp.Entities.Products;
using Acme.SimpleTaskApp.Orders;
using Acme.SimpleTaskApp.Authorization.Users;

namespace Acme.SimpleTaskApp.Cart
{
    public class CartAppService : ApplicationService, ICartAppService
    {
        private readonly IRepository<Cart, long> _cartRepository;
        private readonly IRepository<CartItem, long> _cartItemRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<Order, long> _orderRepository;
        private readonly IRepository<OrderItem, long> _orderItemRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAbpSession _abpSession;

        public CartAppService(
            IRepository<Cart, long> cartRepository,
            IRepository<CartItem, long> cartItemRepository,
            IRepository<Product, int> productRepository,
            IRepository<Order, long> orderRepository,
            IRepository<OrderItem, long> orderItemRepository,
            IRepository<User, long> userRepository,
            IAbpSession abpSession)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _userRepository = userRepository;
            _abpSession = abpSession;
        }

        public async Task<CartDto> GetCart()
        {
            var userId = _abpSession.UserId.Value;
            var cart = await _cartRepository
                .GetAll()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.InsertAsync(cart);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            var cartDto = ObjectMapper.Map<CartDto>(cart);
            cartDto.TotalPrice = cart.Items.Sum(item => item.Price * item.Quantity);

            // Lấy thông tin sản phẩm cho mỗi item trong giỏ hàng
            foreach (var item in cartDto.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                item.ProductName = product.Name;
            }

            return cartDto;
        }

        public async Task AddToCart(AddToCartInput input)
        {
            try
            {
                if (input == null)
                {
                    throw new ApplicationException("Dữ liệu đầu vào không hợp lệ");
                }

                if (input.Quantity <= 0)
                {
                    throw new ApplicationException("Số lượng sản phẩm phải lớn hơn 0");
                }

                var userId = _abpSession.UserId;
                if (!userId.HasValue)
                {
                    throw new ApplicationException("Người dùng chưa đăng nhập");
                }

                var cart = await _cartRepository
                    .GetAll()
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId.Value };
                    await _cartRepository.InsertAsync(cart);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                var product = await _productRepository.GetAsync(input.ProductId);
                if (product == null)
                {
                    throw new ApplicationException("Sản phẩm không tồn tại");
                }
                
                // Kiểm tra số lượng sản phẩm
                if (product.Quantity <= 0)
                {
                    throw new ApplicationException($"Sản phẩm {product.Name} đã hết hàng");
                }

                // Kiểm tra số lượng đặt mua có vượt quá số lượng còn lại không
                if (input.Quantity > product.Quantity)
                {
                    throw new ApplicationException($"Số lượng sản phẩm {product.Name} không đủ. Số lượng còn lại: {product.Quantity}");
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);

                if (existingItem != null)
                {
                    // Kiểm tra tổng số lượng sau khi cộng thêm
                    if (existingItem.Quantity + input.Quantity > product.Quantity)
                    {
                        throw new ApplicationException($"Số lượng sản phẩm {product.Name} không đủ. Số lượng còn lại: {product.Quantity}");
                    }

                    existingItem.Quantity += input.Quantity;
                    existingItem.LastModificationTime = DateTime.Now;
                    await _cartItemRepository.UpdateAsync(existingItem);
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = input.ProductId,
                        Quantity = input.Quantity,
                        Price = product.Price
                    };
                    await _cartItemRepository.InsertAsync(cartItem);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (ApplicationException ex)
            {
                throw ex; // Re-throw các lỗi đã được xử lý
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng. Vui lòng thử lại sau.", ex);
            }
        }

        public async Task UpdateCartItem(UpdateCartItemInput input)
        {
            var userId = _abpSession.UserId.Value;
            var cart = await _cartRepository
                .GetAll()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity = input.Quantity;
                cartItem.LastModificationTime = DateTime.Now;
                await _cartItemRepository.UpdateAsync(cartItem);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCart(RemoveFromCartInput input)
        {
            var userId = _abpSession.UserId.Value;
            var cart = await _cartRepository
                .GetAll()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);
            if (cartItem != null)
            {
                await _cartItemRepository.DeleteAsync(cartItem);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task Checkout()
        {
            var userId = _abpSession.UserId.Value;
            var cart = await _cartRepository
                .GetAll()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new ApplicationException("Cart is empty");
            }

            var user = await _userRepository.GetAsync(userId);

            // Tạo đơn hàng mới
            var order = new Order
            {
                UserId = userId,
                UserName = user.Name,
                UserEmail = user.EmailAddress,
                TotalAmount = cart.Items.Sum(item => item.Price * item.Quantity),
                Status = OrderStatus.Pending,
            };

            var id = await _orderRepository.InsertAndGetIdAsync(order);

            try
            {
                // Thêm các sản phẩm vào đơn hàng và cập nhật số lượng
                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetAsync(cartItem.ProductId);
                    
                    // Kiểm tra số lượng sản phẩm còn đủ không
                    if (product.Quantity < cartItem.Quantity)
                    {
                        throw new ApplicationException($"Sản phẩm {product.Name} không đủ số lượng. Số lượng còn lại: {product.Quantity}");
                    }

                    // Cập nhật số lượng sản phẩm
                    product.Quantity -= cartItem.Quantity;
                    await _productRepository.UpdateAsync(product);

                    var orderItem = new OrderItem
                    {
                        OrderId = id,
                        ProductId = cartItem.ProductId,
                        ProductName = product.Name,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price
                    };

                    await _orderItemRepository.InsertAsync(orderItem);
                }

                // Xóa giỏ hàng
                foreach (var item in cart.Items)
                {
                    await _cartItemRepository.DeleteAsync(item);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, rollback transaction
                await _orderRepository.DeleteAsync(order);
                throw new ApplicationException("Đã xảy ra lỗi khi thanh toán. Vui lòng thử lại sau.", ex);
            }
        }
    }
} 