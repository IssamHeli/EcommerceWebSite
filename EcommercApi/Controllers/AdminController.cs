using Microsoft.AspNetCore.Http;
using EcommercApi.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;  // For BCrypt password hashing and verification
using System.IdentityModel.Tokens.Jwt;  // For working with JWT
using System.Security.Claims;  // For Claims
using Microsoft.IdentityModel.Tokens;  // For SecurityKey, SigningCredentials, and other JWT-related classes
using System.Text;  // For Encoding
using Microsoft.AspNetCore.Authorization;

using EcommercApi.Data;

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly EcommerceDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(EcommerceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("secure-endpoint")]
        [Authorize(Roles = "Admin")] // Only accessible by authenticated admins
        public IActionResult SecureEndpoint()
        {
            return Ok("You have accessed a secure endpoint.");
        }

        

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Username == loginDto.Username);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(admin);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(Admin admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
