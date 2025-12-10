using Microsoft.AspNetCore.Mvc;
using CinemaApi.Data;
using CinemaApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CinemaApi.Models;
using System.Security.Claims;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ReservationController(ApplicationDbContext db) => _db = db;

        [HttpGet("showtime/{showtimeId}/seats")]
        public async Task<IActionResult> GetSeats(int showtimeId)
        {
            var seats = await _db.Seats.Where(s => s.ShowtimeId == showtimeId)
                                       .Select(s => new { s.Id, s.Row, s.Number, s.IsReserved })
                                       .ToListAsync();
            return Ok(seats);
        }

        [Authorize]
        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve(ReservationDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var showtime = await _db.Showtimes.Include(s => s.Room).FirstOrDefaultAsync(s => s.Id == dto.ShowtimeId);
            if (showtime == null) return NotFound(new { message = "Showtime no encontrado" });

            // Lock seats in a simple transactional way
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var selected = new List<Seat>();
                foreach (var sel in dto.Seats)
                {
                    var seat = await _db.Seats
                        .Where(s => s.ShowtimeId == dto.ShowtimeId && s.Row == sel.Row && s.Number == sel.Number)
                        .FirstOrDefaultAsync();

                    if (seat == null)
                    {
                        return BadRequest(new { message = $"Asiento {sel.Row}{sel.Number} no existe" });
                    }
                    if (seat.IsReserved)
                    {
                        return BadRequest(new { message = $"Asiento {sel.Row}{sel.Number} ya reservado" });
                    }

                    seat.IsReserved = true;
                    selected.Add(seat);
                    _db.Seats.Update(seat);
                }

                var reservation = new Reservation
                {
                    ShowtimeId = dto.ShowtimeId,
                    UserId = userId,
                    ReservedAt = DateTime.UtcNow,
                    TotalPrice = showtime.Price * selected.Count,
                    Seats = selected
                };

                _db.Reservations.Add(reservation);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Reserva creada",
                    reservationId = reservation.Id,
                    total = reservation.TotalPrice,
                    seats = selected.Select(s => new { s.Row, s.Number })
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error al reservar", detail = ex.Message });
            }
        }
    }
}
