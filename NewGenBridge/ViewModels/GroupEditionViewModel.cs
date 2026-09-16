using Microsoft.AspNetCore.Mvc.Rendering;
using NewGenBridge.Models;

namespace NewGenBridge.ViewModels
{
    public class GroupEditionViewModel
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan Time { get; set; }
        public int GroupId { get; set; }
        public string MentorId { get; set; }
        public string? Name { get; set; }
        public SelectList? Mentors { get; set; }

        public GroupEditionViewModel(){}
        public GroupEditionViewModel(Group group)
        {
            Day = group.Day;
            Time = group.Time;
            GroupId = group.Id;
            MentorId = group.MentorId ?? "";
            Name = group.Name ?? "";
        }
    }
}
