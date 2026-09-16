namespace NewGenBridge.ViewModels
{
    public class LessonCreationViewModel
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public IFormFile? File { get; set; }
    }
}
