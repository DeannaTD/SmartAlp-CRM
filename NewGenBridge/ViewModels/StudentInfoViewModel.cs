using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace NewGenBridge.ViewModels
{
    public class StudentInfoViewModel
    {
        public string StudentId { get; set; }
        public int StatusId { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly Started { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Note { get; set; }
        public int GroupId { get; set; }
        public int? LevelId { get; set; }
        public string? LevelName { get; set; }
    }
}
