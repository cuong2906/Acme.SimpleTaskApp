using Abp.AutoMapper;
using Acme.SimpleTaskApp.Entities.Products;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Acme.SimpleTaskApp.Products.Dto
{
    [AutoMap(typeof(Product))]

    public class ProductListDto
    {
        [Required]
        [StringLength(Product.MaxNameLength)]
        public string Name { get; set; }
        [Required]

        public decimal Price { get; set; }
        //[Required]

        public IFormFile ImageUrl { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int CategoryId { get; set; }
        public string NameCategory { get; set; }



    }
}
