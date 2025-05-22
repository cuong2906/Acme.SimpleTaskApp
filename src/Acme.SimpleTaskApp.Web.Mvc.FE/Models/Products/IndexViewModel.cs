using Microsoft.AspNetCore.Mvc.Rendering;
using Acme.SimpleTaskApp.Products.Dto;
using System.Collections.Generic;
using System;

namespace Acme.SimpleTaskApp.Web.Models.Products
{
    public class IndexViewModel
    {
        public IReadOnlyList<ProductDto> Products { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public ProductDto Product { get; set; }

        // Pagination properties
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Filter properties
        public string CurrentKeyword { get; set; }
        public decimal? CurrentMinPrice { get; set; }
        public decimal? CurrentMaxPrice { get; set; }
        public int? CurrentCategoryId { get; set; }

        public IndexViewModel(IReadOnlyList<ProductDto> products, List<SelectListItem> categories)
        {
            Products = products;
            Categories = categories;
            TotalCount = products?.Count ?? 0;
        }

        public IndexViewModel()
        {
            PageSize = 10;
            CurrentPage = 1;
            TotalCount = 0;
        }
    }
}
