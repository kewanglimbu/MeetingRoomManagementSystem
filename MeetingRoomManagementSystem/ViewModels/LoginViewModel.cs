using System.ComponentModel.DataAnnotations;

namespace MeetingRoomManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Rememeber me ?")]
        public bool RememberMe { get; set; }
    }
}
