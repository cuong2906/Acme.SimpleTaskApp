using Microsoft.AspNetCore.Mvc.Rendering;
using Acme.SimpleTaskApp.Orders.Dto;
using System.Collections.Generic;

namespace Acme.SimpleTaskApp.Web.Models.Orders
{
    public class OrdersViewModel
    {
        public IReadOnlyList<OrderDto> Orders { get; set; }
        public OrderDto Order { get; set; }

        public OrdersViewModel(IReadOnlyList<OrderDto> orders)
        {
            Orders = orders;
        }
        public OrdersViewModel()
        {

        }
    }
} 