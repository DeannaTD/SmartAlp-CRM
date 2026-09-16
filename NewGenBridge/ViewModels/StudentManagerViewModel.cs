namespace NewGenBridge.ViewModels
{
    public class StudentManagerViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public int StatusId { get; set; }

        public DateOnly StartedOn { get; set; } 
    }
}