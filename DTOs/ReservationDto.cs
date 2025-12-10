using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs
{
    public class SeatSelectionDto
    {
        [Required] public string Row { get; set; }
        [Required] public int Number { get; set; }
    }

    public class ReservationDto
    {
        [Required] public int ShowtimeId { get; set; }
        [Required] public List<SeatSelectionDto> Seats { get; set; }
    }
}
