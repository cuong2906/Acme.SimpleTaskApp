using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Acme.SimpleTaskApp.Entities.Products;

namespace Acme.SimpleTaskApp.Products.Dto
{
    [AutoMap(typeof(Product))]
    public class ProductDto : AuditedEntityDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string NameCategory { get; set; }
        public int Quantity { get; set; }
    }
}
