using Domain.Entities.UsersModule;
using Domain.Models.Auth;

namespace Domain.IContracts.IAuth
{
    public interface IJwtTokenGenerator
    {
        UserTokens GenerateToken(User thisUser);
    }
}
