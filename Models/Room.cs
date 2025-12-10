using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public int Rows { get; set; } = 5;
        public int SeatsPerRow { get; set; } = 8;
    }
}
