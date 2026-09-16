using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class UserAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public int? BranchId { get; set; }
        public int? DivisionId { get; set; }

        public virtual NPUser User { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Division Division { get; set; }
    }
}
