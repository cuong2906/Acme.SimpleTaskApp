using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Acme.SimpleTaskApp.Orders.Dto;

namespace Acme.SimpleTaskApp.Orders
{
    public interface IOrderAppService : IApplicationService
    {
        Task<OrderDto> GetOrder(long id);
        Task<PagedResultDto<OrderDto>> GetAllOrders(PagedOrderResultRequestDto input);
        Task UpdateOrderStatus(UpdateOrderStatusInput input);
        Task DeleteOrder(long id);
    }
} 