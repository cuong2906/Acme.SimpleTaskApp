using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Acme.SimpleTaskApp.Orders
{
    public class OrderItem : Entity<long>, IHasCreationTime, IHasModificationTime
    {
        public long OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public virtual Order Order { get; set; }

        public OrderItem()
        {
            CreationTime = DateTime.Now;
        }
    }
} 