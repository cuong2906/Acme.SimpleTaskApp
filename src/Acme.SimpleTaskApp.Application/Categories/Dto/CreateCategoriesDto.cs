using Abp.AutoMapper;
using Acme.SimpleTaskApp.Entities.Categories;
using System.ComponentModel.DataAnnotations;

namespace Acme.SimpleTaskApp.Categories.Dto
{
    [AutoMap(typeof(Category))]

    public class CreateCategoriesDto
    {
        public int Id { get; set; }

        [Required]

        public string NameCategory { get; set; }

    }
}
