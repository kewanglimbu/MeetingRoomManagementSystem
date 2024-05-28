using Microsoft.AspNetCore.Mvc.Rendering;

namespace MeetingRoomManagementSystem.ViewModels
{
    public class AddParticipantViewModel
    {
        public int BookingId { get; set; }
        public IEnumerable<SelectListItem> ?ParticipantList { get; set; }
        public List<string>? ParticipantIds { get; set; }
        public int CurrentParticipantsCount { get; set; }
        public int RoomCapacity { get; set; }
    }
}
