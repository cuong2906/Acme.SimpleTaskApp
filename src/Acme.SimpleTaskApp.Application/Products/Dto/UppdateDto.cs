using Acme.SimpleTaskApp.Entities.Products;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Acme.SimpleTaskApp.Products.Dto
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(Product.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public IFormFile ImageUrl { get; set; } // Có thể giữ ảnh cũ hoặc upload ảnh mới

        public string ExistingImageUrl { get; set; } // Lưu ảnh cũ

        public int CategoryId { get; set; }
        public string NameCategory { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

}
