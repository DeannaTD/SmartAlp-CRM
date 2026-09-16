namespace NewGenBridge.Models
{
    public class HubFileRole
    {
        public int FileId { get; set; }
        public string RoleName { get; set; }

        public virtual HubFile HubFile { get; set; }
    }
}
