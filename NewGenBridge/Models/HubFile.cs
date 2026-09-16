using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class HubFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public string FilePath { get; set; }
        public string UploadedById { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool? IsKBase { get; set; }

        public virtual NPUser UploadedBy { get; set; }
        public virtual ICollection<HubFileRole> HubFileRoles { get; set; }
    }
}
