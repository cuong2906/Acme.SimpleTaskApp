using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Orders.Dto
{
    public class UpdateOrderStatusInput : EntityDto<long>
    {
        public OrderStatus Status { get; set; }
        public string Note { get; set; }
    }
}   