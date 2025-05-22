using System.Collections.Generic;
using Acme.SimpleTaskApp.Products.Dto;

namespace Acme.SimpleTaskApp.Web.Models.Home
{
    public class IndexViewModel
    {
        public List<ProductDto> TopProducts { get; set; }

        public IndexViewModel()
        {
            TopProducts = new List<ProductDto>();
        }
    }
} 