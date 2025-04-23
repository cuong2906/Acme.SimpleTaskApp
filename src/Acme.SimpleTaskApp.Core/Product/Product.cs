using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Acme.SimpleTaskApp.Entities.Categories;

namespace Acme.SimpleTaskApp.Entities.Products
{
    [Table("AppProducts")]
    public class Product : AuditedEntity
    {

        public const int MaxNameLength = 256;

        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }


        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int Quantity { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        public int? CategoryId { get; set; }


        public Product(string name, decimal price, string imageUrl, int quantity, int? categoryId = null)
        {
            Name = name;
            Price = price;
            ImageUrl = imageUrl;
            Quantity = quantity;
            CategoryId = categoryId;
        }

    }
}
