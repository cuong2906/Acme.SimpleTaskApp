using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Acme.SimpleTaskApp.Cart;
using Acme.SimpleTaskApp.Cart.Dto;
using Acme.SimpleTaskApp.Web.Models.Cart;
using Acme.SimpleTaskApp.Controllers;
using System.Linq;
using Acme.SimpleTaskApp.Entities.Products;
using Abp.Domain.Repositories;

namespace Acme.SimpleTaskApp.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class CartController : SimpleTaskAppControllerBase
    {
        private readonly ICartAppService _cartAppService;
        private readonly IRepository<Product, int> _productRepository;

        public CartController(
            ICartAppService cartAppService,
            IRepository<Product, int> productRepository)
        {
            _cartAppService = cartAppService;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cartDto = await _cartAppService.GetCart();
            
            // Lấy thông tin chi tiết sản phẩm cho mỗi item trong giỏ hàng
            var model = new CartViewModel
            {
                Items = cartDto.Items.Select(item => new CartItemViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.Price * item.Quantity
                }).ToList(),
                TotalPrice = cartDto.TotalPrice,
                UserId = cartDto.UserId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddToCart([FromBody] AddToCartInput input)
        {
            await _cartAppService.AddToCart(input);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoveFromCart([FromBody] RemoveFromCartInput input)
        {
            await _cartAppService.RemoveFromCart(input);
            return Json(new { success = true });
        }
    }
}