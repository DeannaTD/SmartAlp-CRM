using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.ViewModels
{
    public class StudentRegistrationViewModel
    {
        //ToDo: добавить валидацию
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string StudentPhoneNumber { get; set; }
        public DateOnly DateStarted { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? Note { get; set; }
        public int GroupId { get; set; }
    }
}