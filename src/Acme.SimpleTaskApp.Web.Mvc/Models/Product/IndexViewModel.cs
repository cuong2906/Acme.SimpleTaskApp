using Microsoft.AspNetCore.Mvc.Rendering;
using Acme.SimpleTaskApp.Products.Dto;
using System.Collections.Generic;

namespace Acme.SimpleTaskApp.Web.Models.Products
{
    public class IndexViewModel
    {
        public IReadOnlyList<ProductDto> Products { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public ProductDto Product { get; set; }

        public IndexViewModel(IReadOnlyList<ProductDto> products, List<SelectListItem> categories)
        {
            Products = products;
            Categories = categories;
        }

        public IndexViewModel()
        {
        }
    }
}
