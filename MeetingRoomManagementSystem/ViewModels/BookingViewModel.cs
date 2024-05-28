using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomManagementSystem.ViewModels
{
    public class BookingViewModel
    {
        //private const string DateTimeFormat = "yyyy-MM-dd HH:mm"; // Customize the format as needed

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }
        public IEnumerable<SelectListItem>? RoomList { get; set; }

        [Required]
        public List<string>? ParticipantIds { get; set; }
        public IEnumerable<SelectListItem>?ParticipantList { get; set; }


    }
}
