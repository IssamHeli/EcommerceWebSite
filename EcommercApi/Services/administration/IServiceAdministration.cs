using EcommercApi.Models;
using System.Threading.Tasks;
using EcommercApi.DtoClasses;
using EcommercApi.Models;

using Microsoft.AspNetCore.Mvc;
public interface IServiceAdministration 
{
    Task<string> GenerateJwtTokenAsync(Admin admin);
    Task<LoginResult> LoginAsync(LoginDto loginDto);
}
