using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string PasswordHash { get; set; }
        public string Role { get; set; } = "User"; // or "Admin"
    }
}
