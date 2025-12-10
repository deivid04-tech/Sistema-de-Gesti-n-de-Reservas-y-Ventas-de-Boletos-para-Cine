namespace CinemaApi.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; }

        public string Row { get; set; } // "A", "B"
        public int Number { get; set; } // 1..N

        public bool IsReserved { get; set; } = false;
        public int? ReservationId { get; set; }
    }
}
