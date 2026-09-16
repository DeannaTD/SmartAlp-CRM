using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username or email")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public LoginViewModel()
        {
            RememberMe = false;
        }
    }
}
