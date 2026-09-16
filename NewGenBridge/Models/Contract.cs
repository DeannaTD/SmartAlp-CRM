using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace NewGenBridge.Models
{
    public class Contract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string Number { get; set; }
        public int BranchId { get; set; }
        public int? StudentId { get; set; }
        public DateTime SignDate { get; set; }
        public string? StudentName { get; set; }
        public string? ParentName { get; set; }
        public int PriceId { get; set; }
        public decimal? PriceAmount { get; set; }

        public virtual Price Price { get; set; }

        [NotMapped]
        public DateTime ExpirationDate => SignDate.AddMonths(6);
        
        [ValidateNever]
        public virtual IEnumerable<Bill> Bills { get; set; }
    }
}
