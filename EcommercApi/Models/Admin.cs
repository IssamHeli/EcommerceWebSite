
namespace EcommercApi.Models;

public class Admin
{
    public int Id { get; set; } // Primary Key
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Store hashed passwords
}
