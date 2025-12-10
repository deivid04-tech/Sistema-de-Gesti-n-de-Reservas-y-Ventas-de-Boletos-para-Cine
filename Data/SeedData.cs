using CinemaApi.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaApi.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher<User> hasher)
        {
            if (!db.Users.Any())
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin@example.com",
                    Role = "Admin"
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
                db.Users.Add(admin);

                var user = new User
                {
                    Name = "Test User",
                    Email = "user@example.com",
                    Role = "User"
                };
                user.PasswordHash = hasher.HashPassword(user, "User123!");
                db.Users.Add(user);
            }

            if (!db.Movies.Any())
            {
                var m1 = new Movie { Title = "La Aventura", Description = "Película de aventura", DurationMinutes = 120 };
                var m2 = new Movie { Title = "El Misterio", Description = "Thriller emocionante", DurationMinutes = 100 };
                db.Movies.AddRange(m1, m2);
            }

            if (!db.Rooms.Any())
            {
                var r1 = new Room { Name = "Sala 1", Rows = 5, SeatsPerRow = 8 };
                var r2 = new Room { Name = "Sala 2", Rows = 4, SeatsPerRow = 6 };
                db.Rooms.AddRange(r1, r2);
            }

            await db.SaveChangesAsync();

            if (!db.Showtimes.Any())
            {
                var movie1 = db.Movies.First();
                var room1 = db.Rooms.First();

                var st1 = new Showtime { MovieId = movie1.Id, RoomId = room1.Id, StartTime = DateTime.UtcNow.AddHours(3), Price = 6.0m };
                db.Showtimes.Add(st1);
                await db.SaveChangesAsync();

                // generate seats for this showtime
                var room = db.Rooms.First(r => r.Id == st1.RoomId);
                var seats = new List<Seat>();
                for (int r = 0; r < room.Rows; r++)
                {
                    var rowChar = ((char)('A' + r)).ToString();
                    for (int n = 1; n <= room.SeatsPerRow; n++)
                    {
                        seats.Add(new Seat { ShowtimeId = st1.Id, Row = rowChar, Number = n });
                    }
                }
                db.Seats.AddRange(seats);
            }

            await db.SaveChangesAsync();
        }
    }
}
