using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Acme.SimpleTaskApp.Cart
{
    public class Cart : Entity<long>, IHasCreationTime, IHasModificationTime
    {
        public long UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public virtual ICollection<CartItem> Items { get; set; }

        public Cart()
        {
            CreationTime = DateTime.Now;
            Items = new List<CartItem>();
        }
    }
} 