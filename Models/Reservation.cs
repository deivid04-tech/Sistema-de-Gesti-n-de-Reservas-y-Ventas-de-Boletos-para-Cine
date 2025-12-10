using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public decimal TotalPrice { get; set; }
    }
}
