namespace NewGenBridge.ViewModels
{
    public class StaffRegistrationViewModel
    {
        //ToDo: Добавить валидацию (имя и фамилия только алфавит, дата рождения не позже сегодняшнего дня)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Role { get; set; }
        public int DivisionId { get; set; }
    }
}
