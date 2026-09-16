using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class KitRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int Amount { get; set; }
        public int Status { get; set; } //New, Pending, Completed
        public int CompanyId { get; set; }
        public string CreatedById { get; set; }
        public int? LevelId { get; set; }

        [ValidateNever]
        public virtual Company Company { get; set; }

    }
}
