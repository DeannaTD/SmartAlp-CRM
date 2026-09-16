using NewGenBridge.Models;

namespace NewGenBridge.ViewModels
{
    public class StudentAttendanceViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<Attendance> Attendances { get; set; }
    }
}
