using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; } = 5.00m;

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
