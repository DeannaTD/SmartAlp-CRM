using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class Royalty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public int BranchId { get; set; }
        public DateTime Period { get; set; }
        public double Percentage { get; set; }
        public bool IsPaid { get; set; }
        public decimal Total { get; set; }

        [ValidateNever]
        public virtual Branch Branch { get; set; }
    }
}
