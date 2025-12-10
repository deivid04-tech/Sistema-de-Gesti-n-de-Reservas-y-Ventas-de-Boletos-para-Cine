using Microsoft.EntityFrameworkCore;
using CinemaApi.Models;

namespace CinemaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<Seat>()
                .HasIndex(s => new { s.RoomId, s.Row, s.Number })
                .IsUnique();

            // relationships
            builder.Entity<Showtime>()
                .HasMany(s => s.Seats)
                .WithOne(s => s.Showtime)
                .HasForeignKey(s => s.ShowtimeId);
        }
    }
}
