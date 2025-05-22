using Microsoft.AspNetCore.Mvc.Rendering;
using Acme.SimpleTaskApp.Orders.Dto;
using Acme.SimpleTaskApp.Orders;
using System.Collections.Generic;

namespace Acme.SimpleTaskApp.Web.Models.Orders
{
    public class IndexViewModel
    {
        public IReadOnlyList<OrderDto> Orders { get; set; }
        public OrderDto Order { get; set; }

        public IndexViewModel(IReadOnlyList<OrderDto> orders)
        {
            Orders = orders;
        }

        public IndexViewModel()
        {
        }
    }
} 