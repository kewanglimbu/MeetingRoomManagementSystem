using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomManagementSystem.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
        //public IEnumerable<string> Roles { get; set; }

    }
}
