using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? TimeZoneId { get; set; }

        [ValidateNever]
        public virtual ICollection<Group> Groups { get; set; }

        [NotMapped]
        public TimeZoneInfo TimeZone => !string.IsNullOrEmpty(TimeZoneId)
                                        ? TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId)
                                        : TimeZoneInfo.Utc;


    }
}
