using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.SimpleTaskApp.Categories.Dto
{
    public class PagedCategoriesDto:PagedAndSortedResultRequestDto
    {
        public PagedCategoriesDto()
        {
            MaxResultCount = 15;
        }

        public string Keyword { get; set; }
    }
}
