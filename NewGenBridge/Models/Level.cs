using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.Models
{
    public class Level
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ValidateNever]
        public virtual ICollection<Lesson> Lessons { get; set; }

    }
}
