using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Orders.Dto
{
    public class PagedOrderResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public OrderStatus? Status { get; set; }
        public long? UserId { get; set; }
    }
} 