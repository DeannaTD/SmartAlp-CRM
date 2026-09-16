using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ContractId { get; set; }
        public bool Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Sum { get; set; }
        public string BillNumber { get; set; }
        public DateTime? DueDate { get; set; }

        [ValidateNever]
        public virtual Contract Contract { get; set; }
    }
}
