using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Acme.SimpleTaskApp.Orders.Dto;
using Acme.SimpleTaskApp.Products.Dto;
using System.Collections.Generic;

namespace Acme.SimpleTaskApp.Orders
{
    public interface IOrderAppService : IApplicationService
    {
        Task<OrderDto> GetOrder(long id);
        Task<PagedResultDto<OrderDto>> GetAllOrders(PagedOrderResultRequestDto input);
        Task UpdateOrderStatus(UpdateOrderStatusInput input);
        Task DeleteOrder(long id);
        Task<List<ProductDto>> GetTopProductsByOrderQuantity(int count = 5);
    }
} 