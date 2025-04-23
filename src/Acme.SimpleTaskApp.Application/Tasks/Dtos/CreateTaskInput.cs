using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace Acme.SimpleTaskApp.Tasks.Dtos
{
    [AutoMapTo(typeof(Tasks))]
    public class CreateTaskInput
    {
        [Required]
        [MaxLength(Tasks.MaxTitleLength)]
        public string Title { get; set; }

        [MaxLength(Tasks.MaxDescriptionLength)]
        public string Description { get; set; }

        public Guid? AssignedPersonId { get; set; }
    }
}