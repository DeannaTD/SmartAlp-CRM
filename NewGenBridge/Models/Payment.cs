using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PaymentPeriodId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }
        public DateTime DateofPayment { get; set; }

        public virtual PaymentPeriod PaymentPeriod { get; set; }
    }
}
