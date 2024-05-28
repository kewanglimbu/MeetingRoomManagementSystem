using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomSys.Models
{
    public class Participant
    {
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
