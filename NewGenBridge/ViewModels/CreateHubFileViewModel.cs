namespace NewGenBridge.ViewModels
{
    public class CreateHubFileViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public IEnumerable<string> SelectedRoles { get; set; } = new List<string>();
    }
}
