using Microsoft.AspNetCore.Mvc.Rendering;
using NewGenBridge.Models;

namespace NewGenBridge.ViewModels
{
    public class GroupCreationViewModel
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan Time { get; set; }
        public int BranchId { get; set; }
        public string MentorId { get; set; }
        public string? Name { get; set; }
        public SelectList? Mentors { get; set; }
    }
}
