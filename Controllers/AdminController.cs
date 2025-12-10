using Microsoft.AspNetCore.Mvc;
using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) => _db = db;

        // Movies CRUD
        [HttpPost("movies")]
        public async Task<IActionResult> CreateMovie(Movie movie)
        {
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
            return Ok(movie);
        }

        [HttpPut("movies/{id}")]
        public async Task<IActionResult> UpdateMovie(int id, Movie data)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            movie.Title = data.Title;
            movie.Description = data.Description;
            movie.DurationMinutes = data.DurationMinutes;
            movie.Active = data.Active;
            await _db.SaveChangesAsync();
            return Ok(movie);
        }

        [HttpDelete("movies/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // Rooms
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom(Room room)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
            return Ok(room);
        }

        // Showtimes
        [HttpPost("showtimes")]
        public async Task<IActionResult> CreateShowtime(Showtime st)
        {
            // create seats for showtime
            _db.Showtimes.Add(st);
            await _db.SaveChangesAsync();

            var room = await _db.Rooms.FindAsync(st.RoomId);
            var seats = new List<Seat>();
            for (int r = 0; r < room.Rows; r++)
            {
                var rowChar = ((char)('A' + r)).ToString();
                for (int n = 1; n <= room.SeatsPerRow; n++)
                {
                    seats.Add(new Seat { ShowtimeId = st.Id, Row = rowChar, Number = n });
                }
            }
            _db.Seats.AddRange(seats);
            await _db.SaveChangesAsync();

            return Ok(st);
        }
    }
}
