using System;
using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Cart.Dto
{
    public class CartItemDto : EntityDto<long>
    {
        public long CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
} 