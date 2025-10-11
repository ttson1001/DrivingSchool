using BEAPI.Entities;
using System.Security.Claims;

namespace BEAPI.Services.IServices
{
    public interface IJwtService
    {
        string GenerateToken(Account account   , int? expiresInMinutes);
        ClaimsPrincipal? ValidateToken(string token);
    }
}