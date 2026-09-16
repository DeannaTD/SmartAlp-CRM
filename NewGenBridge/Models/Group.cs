using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace NewGenBridge.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan Time { get; set; }
        public int BranchId { get; set; }

        public int? lessonId { get; set; }

        public string? Name { get; set; }
        public string? MentorId { get; set; }

        [ValidateNever]
        public virtual ICollection<NPUser> Users { get; set; }
        public virtual Lesson? Lesson { get; set; }
    }
}
