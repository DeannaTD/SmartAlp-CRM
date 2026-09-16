using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class Lead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public int LevelId { get; set; }
        public int BranchId { get; set; }
        public int? ContractId { get; set; }
    }
}
