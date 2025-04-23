using System.Collections.Generic;

namespace Acme.SimpleTaskApp.Web.Models.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public long UserId { get; set; }

        public CartViewModel()
        {
            Items = new List<CartItemViewModel>();
        }
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ImageUrl { get; set; }
    }
} 