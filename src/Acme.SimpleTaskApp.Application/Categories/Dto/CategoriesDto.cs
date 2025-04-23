using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Acme.SimpleTaskApp.Entities.Categories;
using System.ComponentModel.DataAnnotations;

namespace Acme.SimpleTaskApp.Categories.Dto
{
    [AutoMap(typeof(Category))]
    public class CategoriesDto : AuditedEntityDto
    {
        [Required]
        public string NameCategory { get; set; }
    }
}
