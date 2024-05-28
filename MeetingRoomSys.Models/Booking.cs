using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MeetingRoomSys.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        [ForeignKey("Organizer")]
        public string OrganizerId { get; set; }
        public ApplicationUser Organizer { get; set; }
        // Define the Participants property here.
        public ICollection<Participant> Participants { get; set; }
    }
}

