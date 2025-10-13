using TutorDrive.Entities;
using System.Security.Claims;

namespace TutorDrive.Services.IServices
{
    public interface IJwtService
    {
        string GenerateToken(Account account   , int? expiresInMinutes);
        ClaimsPrincipal? ValidateToken(string token);
    }
}