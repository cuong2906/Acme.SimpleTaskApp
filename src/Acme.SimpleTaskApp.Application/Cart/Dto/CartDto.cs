using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Acme.SimpleTaskApp.Cart.Dto
{
    public class CartDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 