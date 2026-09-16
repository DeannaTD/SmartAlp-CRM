namespace NewGenBridge.ViewModels
{
    public class MentorLevelAccessViewModel
    {
        public string MentorId { get; set; }
        public string MentorName { get; set; }
        public List<LevelAccess> LevelAccesses { get; set; }
    }

    public class LevelAccess
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public bool HasAccess { get; set; }

    }
}
