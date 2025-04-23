using System.Collections.Generic;
using Acme.SimpleTaskApp.Categories.Dto;

namespace Acme.SimpleTaskApp.Web.Models.Categories
{
    public class IndexViewModel
    {
        public IReadOnlyList<CategoriesDto> Categories { get; set; }
        public CategoriesDto Category { get; set; }

        public IndexViewModel(IReadOnlyList<CategoriesDto> categories)
        {
            Categories = categories;
        }

        public IndexViewModel()
        {
        }
    }
}
