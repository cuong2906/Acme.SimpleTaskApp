using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Cart.Dto
{
    public class AddToCartInput : EntityDto<long>
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
} 