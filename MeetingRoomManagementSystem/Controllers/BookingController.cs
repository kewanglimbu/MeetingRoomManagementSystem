using MeetingRoomManagementSystem.ViewModels;
using MeetingRoomSys.DataAccess;
using MeetingRoomSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeetingRoomManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Basic")]
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomCapacity(int roomId)
        {
            var room = await _context.Rooms
                .Where(r => r.Id == roomId)
                .FirstOrDefaultAsync();

            if (room != null)
            {
                return Ok(room.Capacity);
            }
            else
            {
                return NotFound("Room not found");
            }
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var bookingsQuery = _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Organizer);

            if (isAdmin)
            {
                var adminBookings = bookingsQuery.ToList();
                return View(adminBookings);
            }
            else
            {
                var userBookings = bookingsQuery.Where(b => b.OrganizerId == userId).ToList();
                return View(userBookings);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var availableRooms = _context.Rooms
                .Where(room => room.IsAvailable).ToList();
            var viewModel = new BookingViewModel
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),

                RoomList = availableRooms
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                    .ToList(),

                ParticipantList = _context.Users
                 .Select(u => new SelectListItem { Value = u.Id, Text = u.UserName })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var organizer = _context.Users.Find(organizerId);

                if (organizer == null)
                {
                    ModelState.AddModelError("OrganizerUsername", "Organizer not found");
                }
                else
                {
                    // Check if the selected room exists
                    var selectedRoom = _context.Rooms.Find(viewModel.RoomId);

                    if (selectedRoom == null)
                    {
                        ModelState.AddModelError("RoomId", "Selected room does not exist.");
                    }
                    else
                    {
                        // Check for overlapping bookings for the same room and time slot
                        var overlappingBooking = _context.Bookings
                            .FirstOrDefault(booking =>
                                booking.RoomId == viewModel.RoomId &&
                                viewModel.StartTime < booking.EndTime &&
                                viewModel.EndTime > booking.StartTime);

                        if (overlappingBooking != null)
                        {
                            ModelState.AddModelError("RoomId", "Selected room is already booked for the specified time slot.");
                        }
                        else if (viewModel.ParticipantIds.Count > selectedRoom.Capacity)
                        {
                            ModelState.AddModelError("RoomId", "Selected room does not have enough capacity for all selected Participants.");
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            DateTime StartTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0); // Start of the current day
                            DateTime EndTime = StartTime.AddDays(1).AddSeconds(-1); // End of the current day (last second)

                            var booking = new Booking
                            {
                                Title = viewModel.Title,
                                Description = viewModel.Description,
                                StartTime = viewModel.StartTime,
                                EndTime = viewModel.EndTime,
                                RoomId = viewModel.RoomId,
                                OrganizerId = organizerId
                            };

                            _context.Bookings.Add(booking);
                            _context.SaveChanges();

                            // Handle participants
                            if (viewModel.ParticipantIds != null && viewModel.ParticipantIds.Any())
                            {
                                foreach (var participantId in viewModel.ParticipantIds)
                                {
                                    var participant = new Participant
                                    {
                                        UserId = participantId,
                                        BookingId = booking.Id
                                    };

                                    _context.Participants.Add(participant);
                                }

                                _context.SaveChanges();
                            }

                            return RedirectToAction("Index");
                        }
                    }
                }
            }

            viewModel.RoomList = _context.Rooms
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                .ToList();

            viewModel.ParticipantList = _context.Users
                .Select(u => new SelectListItem { Value = u.Id, Text = u.UserName })
                .ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddParticipant(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Participants)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var currentParticipantsCount = booking.Participants.Count;
            var roomCapacity = booking.Room.Capacity;

            var currentParticipants = booking.Participants.Select(p => p.UserId).ToList();

            var allParticipants = _context.Users.Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            var availableParticipants = allParticipants
                .Where(user => !currentParticipants.Contains(user.Value))
                .ToList();

            var viewModel = new AddParticipantViewModel
            {
                BookingId = id,
                ParticipantList = availableParticipants,
                ParticipantIds = new List<string>(),
                CurrentParticipantsCount = currentParticipantsCount,
                RoomCapacity = roomCapacity
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddParticipant(AddParticipantViewModel viewModel, int bookingId)
        {
            if (ModelState.IsValid)
            {
                var booking = _context.Bookings
                    .Include(b => b.Participants)
                    .Include(b => b.Room)
                    .FirstOrDefault(b => b.Id == bookingId);

                if (booking == null)
                {
                    return NotFound();
                }

                var currentParticipantsCount = booking.Participants.Count;
                var roomCapacity = booking.Room.Capacity;

                if (viewModel.ParticipantIds != null)
                {
                    if (currentParticipantsCount + viewModel.ParticipantIds.Count > roomCapacity)
                    {
                        ModelState.AddModelError("", "Cannot add participants beyond the room's capacity.");
                    }
                    else
                    {
                        foreach (var participantId in viewModel.ParticipantIds)
                        {
                            var participant = new Participant
                            {
                                BookingId = booking.Id,
                                UserId = participantId
                            };
                            _context.Participants.Add(participant);
                        }

                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }

            var currentParticipants = _context.Participants
                .Where(p => p.BookingId == viewModel.BookingId)
                .Select(p => p.UserId)
                .ToList();

            var allParticipants = _context.Users.Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            var availableParticipants = allParticipants
                .Where(user => !currentParticipants.Contains(user.Value))
                .ToList();

            viewModel.ParticipantList = availableParticipants;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookdetail = await _context.Bookings
                    .Include(s => s.Participants)
                    .ThenInclude(p => p.User) // Include the User associated with each Participant
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (bookdetail == null)
            {
                return NotFound();
            }

            return View(bookdetail);
        }
    }
}



