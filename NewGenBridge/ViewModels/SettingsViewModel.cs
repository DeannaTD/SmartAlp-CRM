using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.ViewModels
{
    public class SettingsViewModel
    {
        public string UserId { get; set; }
        //ToDo: добавить валидацию
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}
