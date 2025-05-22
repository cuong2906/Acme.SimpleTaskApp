using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Acme.SimpleTaskApp.Orders.Dto;
using Acme.SimpleTaskApp.Products.Dto;

namespace Acme.SimpleTaskApp.Orders
{
    public class OrderAppService : SimpleTaskAppAppServiceBase, IOrderAppService
    {
        private readonly IRepository<Order, long> _orderRepository;
        private readonly IRepository<OrderItem, long> _orderItemRepository;

        public OrderAppService(
            IRepository<Order, long> orderRepository,
            IRepository<OrderItem, long> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<OrderDto> GetOrder(long id)
        {
            var order = await _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            return ObjectMapper.Map<OrderDto>(order);
        }

        public async Task<PagedResultDto<OrderDto>> GetAllOrders(PagedOrderResultRequestDto input)
        {
            var query = _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                    o => o.UserName.Contains(input.Keyword) ||
                         o.UserEmail.Contains(input.Keyword))
                .WhereIf(input.Status.HasValue, o => o.Status == input.Status.Value)
                .WhereIf(input.UserId.HasValue, o => o.UserId == input.UserId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreationTime)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<OrderDto>(
                totalCount,
                ObjectMapper.Map<List<OrderDto>>(items)
            );
        }

        public async Task UpdateOrderStatus(UpdateOrderStatusInput input)
        {
            var order = await _orderRepository.GetAsync(input.Id);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            order.Status = input.Status;
            order.Note = input.Note;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrder(long id)
        {
            var order = await _orderRepository.GetAll()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            foreach (var item in order.OrderItems)
            {
                await _orderItemRepository.DeleteAsync(item);
            }

            await _orderRepository.DeleteAsync(order);
        }

        public async Task<List<ProductDto>> GetTopProductsByOrderQuantity(int count = 5)
        {
            var topProducts = await _orderItemRepository.GetAll()
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(count)
                .ToListAsync();

            return topProducts.Select(p => new ProductDto
            {
                Id = p.ProductId,
                Name = p.ProductName
            }).ToList();
        }
    }
}