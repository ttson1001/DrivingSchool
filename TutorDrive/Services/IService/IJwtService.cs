using TutorDrive.Entities;
using System.Security.Claims;

namespace TutorDrive.Services.IService
{
    public interface IJwtService
    {
        string GenerateToken(Account account   , int? expiresInMinutes);
        ClaimsPrincipal? ValidateToken(string token);
    }
}