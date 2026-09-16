using NewGenBridge.Models;

namespace NewGenBridge.ViewModels
{
    public class AttendanceViewModel
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentUserName { get; set; }
        public bool IsBlocked { get; set; }
        public int LessonId { get; set; }
        public List<Attendance> Attendances { get; set; }
    }
}
