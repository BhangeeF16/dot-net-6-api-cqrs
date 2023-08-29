using Domain.Entities.UsersModule;
using Domain.Models.Auth;
using System.Security.Claims;

namespace Domain.Abstractions.IAuth
{
    public interface IJwtTokenGenerator
    {
        UserTokens GenerateToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string? token);
    }
}
