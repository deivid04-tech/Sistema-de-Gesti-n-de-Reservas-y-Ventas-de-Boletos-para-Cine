using Microsoft.AspNetCore.Mvc;
using CinemaApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public MoviesController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _db.Movies.Where(m => m.Active).ToListAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id && m.Active);
            if (movie == null) return NotFound();
            var showtimes = await _db.Showtimes
                .Where(s => s.MovieId == id && s.StartTime >= DateTime.UtcNow)
                .Include(s => s.Room)
                .ToListAsync();

            return Ok(new { movie, showtimes });
        }
    }
}
