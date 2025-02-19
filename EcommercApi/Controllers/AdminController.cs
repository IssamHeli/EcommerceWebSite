using Microsoft.AspNetCore.Http;
using EcommercApi.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;  // For BCrypt password hashing and verification
using System.IdentityModel.Tokens.Jwt;  // For working with JWT
using System.Security.Claims;  // For Claims
using Microsoft.IdentityModel.Tokens;  // For SecurityKey, SigningCredentials, and other JWT-related classes
using System.Text;  // For Encoding
using Microsoft.AspNetCore.Authorization;
using EcommercApi.DtoClasses;
using EcommercApi.Data;

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IServiceAdministration _IServiceAdministration ;

        public AdminController(IServiceAdministration ServiceAdministration)
        {

            _IServiceAdministration = ServiceAdministration ;
        }


        [HttpGet("secure-endpoint")]
        [Authorize(Roles = "Admin")] // Only accessible by authenticated admins
        public IActionResult SecureEndpoint()
        {
            return Ok("You have accessed a secure endpoint.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _IServiceAdministration.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(new { Token = result.Token });
        }
        
        
    }