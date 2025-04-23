using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Acme.SimpleTaskApp.Cart
{
    public class CartItem : Entity<long>, IHasCreationTime, IHasModificationTime
    {
        public long CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public virtual Cart Cart { get; set; }

        public CartItem()
        {
            CreationTime = DateTime.Now;
        }
    }
} 