using Microsoft.AspNetCore.Http;
using EcommercApi.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;  // For BCrypt password hashing and verification
using System.IdentityModel.Tokens.Jwt;  // For working with JWT
using System.Security.Claims;  // For Claims
using Microsoft.IdentityModel.Tokens;  // For SecurityKey, SigningCredentials, and other JWT-related classes
using System.Text;  // For Encoding
using Microsoft.AspNetCore.Authorization;
using EcommercApi.Services ;
using EcommercApi.Data;
using EcommercApi.DtoClasses ;
using Microsoft.EntityFrameworkCore;
namespace EcommercApi.Services;

public class ServiceAdministration : IServiceAdministration
    {
        private readonly EcommerceDbContext _context;
        private readonly IConfiguration _configuration;

        public ServiceAdministration(EcommerceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtTokenAsync(Admin admin)
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

      public async Task<LoginResult> LoginAsync(LoginDto loginDto)
    {
        var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == loginDto.Username);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.PasswordHash))
        {
            return new LoginResult
            {
                IsSuccess = false,
                ErrorMessage = "Invalid username or password."
            };
        }

        var token = await GenerateJwtTokenAsync(admin);
        return new LoginResult
        {
            IsSuccess = true,
            Token = token
        };
    }

       
      
    }
  
    
    
