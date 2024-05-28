using MeetingRoomSys.DataAccess;
using MeetingRoomSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RoomController : Controller
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext appDbContext)
        {
            _context = appDbContext;

        }

        public IActionResult Index()
        {
            return View(_context.Rooms.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Id,Name,Capacity,IsAvailable")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Capacity,IsAvailable")] Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            var existingRoom = await _context.Rooms
                .Include(r => r.Bookings)
                .ThenInclude(b => b.Participants)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRoom == null)
            {
                return NotFound();
            }

            var currentParticipantCount = existingRoom.Bookings
                .SelectMany(b => b.Participants)
                .Count();

            if (room.Capacity < currentParticipantCount)
            {
                ModelState.AddModelError("Capacity", "New capacity cannot be less than the number of existing participants.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingRoom.Name = room.Name;
                    existingRoom.Capacity = room.Capacity;
                    existingRoom.IsAvailable = room.IsAvailable;

                    _context.Update(existingRoom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .ThenInclude(b => b.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = _context.Rooms.FirstOrDefault(m => m.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var room = _context.Rooms.Find(id);
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
