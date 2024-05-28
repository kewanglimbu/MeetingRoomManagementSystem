using MeetingRoomSys.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomSys.DataAccess
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Configure the relationship between Participant and Booking
            builder.Entity<Participant>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Participants)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between Participant and User
            builder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany() // Assuming there is a relationship, but no cascading delete needed
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }    

        public DbSet<Participant> Participants { get; set; }
    }
}
