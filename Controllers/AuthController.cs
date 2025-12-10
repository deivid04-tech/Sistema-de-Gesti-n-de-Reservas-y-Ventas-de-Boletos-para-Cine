using Microsoft.AspNetCore.Mvc;
using CinemaApi.Data;
using CinemaApi.DTOs;
using CinemaApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext db, IPasswordHasher<User> hasher, IConfiguration config)
        {
            _db = db;
            _hasher = hasher;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "El correo ya está registrado." });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email
            };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Registro exitoso" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized(new { message = "Credenciales inválidas" });

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var token = GenerateToken(user);
            return Ok(new { token, user = new { user.Id, user.Name, user.Email, user.Role } });
        }

        private string GenerateToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key"));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSection.GetValue<string>("Issuer"),
                audience: jwtSection.GetValue<string>("Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("DurationMinutes")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
