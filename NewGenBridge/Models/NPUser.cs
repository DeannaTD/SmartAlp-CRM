using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class NPUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ParentName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfTrial { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateStarted { get; set; }
        public string? StudentPhoneNumber { get; set; }
        public string? Note { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentAmount { get; set; }
        public int? GroupId { get; set; }
        public int? StatusId { get; set; }
        public int? CurrentLessonId { get; set; }
        public int? MentorDivisionId { get; set; }
        public int? MentorLastGroupBranchId { get; set; }

        public virtual IdentityRole Access { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<UserLesson> UserLessons { get; set; }
        public virtual Status Status { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<PaymentPeriod> PaymentPeriods { get; set; }

    }
}