using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Products.Dto
{
    public class PagedProductDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public decimal? MinPrice { get; set; }  // Tham số lọc giá tối thiểu
        public decimal? MaxPrice { get; set; }  // Tham số lọc giá tối đa
        public int? CategoryId { get; set; }    // Tham số lọc theo danh mục
    }
}
