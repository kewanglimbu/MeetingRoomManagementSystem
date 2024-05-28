namespace MeetingRoomManagementSystem.ViewModels
{
    public class UserRolesViewModel
    {
        //public string RoleId { get; set; } = string.Empty;
        //public string RoleName { get; set; }=string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsSelected { get; set; } 
        public IEnumerable<string> Roles { get; set; }

    }
}
