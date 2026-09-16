using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class Price
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BranchId { get; set; }
        public int LevelId { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Sum { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaterialsPrice { get; set; }

        public int DurationInMonths { get; set; } = 6;


        public virtual Branch Branch { get; set; }
        public virtual Level Level { get; set; }
    }
}
