using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.Models
{
    public class Agreement
    {
        [Key]
        public string Id { get; set; }
        public int CompanyId { get; set; }

        public DateTime Created { get; set; }
    }
}
