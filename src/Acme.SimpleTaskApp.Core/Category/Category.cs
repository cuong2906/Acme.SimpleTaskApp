using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Acme.SimpleTaskApp.Entities.Categories
{
    [Table("AppCategories")]
    public class Category : AuditedEntity
    {
        public const int MaxNameLength = 32;

        [Required]
        [StringLength(MaxNameLength)]
        public string NameCategory { get; set; }
    }
}
