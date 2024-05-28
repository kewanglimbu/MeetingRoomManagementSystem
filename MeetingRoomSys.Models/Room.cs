using System.ComponentModel.DataAnnotations;

namespace MeetingRoomSys.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Range(1, 20)]
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<Booking>? Bookings { get; set; } // Collection of bookings for this room
    }
}
