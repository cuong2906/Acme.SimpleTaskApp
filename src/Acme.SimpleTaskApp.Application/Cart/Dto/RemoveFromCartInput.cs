using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Cart.Dto
{
    public class RemoveFromCartInput : EntityDto<long>
    {
        public int ProductId { get; set; }
    }
} 