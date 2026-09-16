namespace NewGenBridge.ViewModels
{
    public class BranchPanelViewModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public List<GroupInBranchViewModel> GroupsInfo { get; set; }
    }

    public struct GroupInBranchViewModel
    {
        public int GroupId { get; set; }
        public string TimeString { get; set; }
        public DayOfWeek Day { get; set; }
        public string MentorName { get; set; }
        public int StudentsCount { get; set; }

        public string GroupName { get; set; }
    }
}
