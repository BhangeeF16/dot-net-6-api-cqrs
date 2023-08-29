using Application.Common.Services;
using Domain.Abstractions.IAuth;
using Domain.Entities.UsersModule;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Application.Pipeline.Authentication.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddHttpContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<ICurrentUserService, CurrentUserService>();

        return services;
    }
    public static List<Claim> GetClaims(this User user)
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.Sid, user?.ID.ToString() ?? string.Empty),
            new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user?.fk_RoleID.ToString() ?? string.Empty),
            //new Claim(ClaimTypes.Name, user?.FirstName ?? string.Empty),
            //new Claim(ClaimTypes.GivenName, user?.ChargeBeeCustomerID ?? string.Empty),
            //new Claim(ClaimTypes.Country, user?.Cin7CustomerID ?? string.Empty),
            //new Claim(ClaimTypes.Actor, ChargeBeeSubscriptionID),
        };
    }
    public static string ExtractToken(this string authorization, string scheme = AuthLegend.Scheme.BEARER)
    {
        if (authorization != null && authorization.StartsWith($"{scheme} ", StringComparison.OrdinalIgnoreCase))
            return authorization[$"{scheme} ".Length..].Trim();
        return authorization ?? string.Empty;
    }
}

#region UNUSED CODE

//public static List<Claim> GetClaims(this User user)
//{
//    UserSubscription userSubscription = null;
//    if (user?.Subscriptions?.Any(x => x.Status == SubscriptionState.Active) ?? false)
//    {
//        userSubscription = user?.Subscriptions?.FirstOrDefault(x => x.Status == SubscriptionState.Active);
//    }
//    else
//    {
//        userSubscription = user?.Subscriptions?.OrderByDescending(x => x.ID).FirstOrDefault();
//    }

//    return new List<Claim>()
//    {
//        new Claim(ClaimTypes.Sid, user?.ID.ToString() ?? string.Empty),
//        new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
//        new Claim(ClaimTypes.Name, user?.FirstName ?? string.Empty),
//        new Claim(ClaimTypes.Role, user?.fk_RoleID.ToString() ?? string.Empty),
//        new Claim(ClaimTypes.GivenName, user?.ChargeBeeCustomerID ?? string.Empty),
//        new Claim(ClaimTypes.Country, user?.Cin7CustomerID ?? string.Empty),
//        new Claim(ClaimTypes.Actor, user?.fk_RoleID == 1
//                                    ? string.Empty
//                                    : userSubscription?.ChargeBeeID ?? string.Empty),
//    };
//}
//public static List<Claim> GetClaims(this User user, string ChargeBeeSubscriptionID)
//{
//    return new List<Claim>()
//    {
//        new Claim(ClaimTypes.Sid, user?.ID.ToString() ?? string.Empty),
//        new Claim(ClaimTypes.Name, user?.FirstName ?? string.Empty),
//        new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
//        new Claim(ClaimTypes.Role, user?.fk_RoleID.ToString() ?? string.Empty),
//        new Claim(ClaimTypes.GivenName, user?.ChargeBeeCustomerID ?? string.Empty),
//        new Claim(ClaimTypes.Country, user?.Cin7CustomerID ?? string.Empty),
//        new Claim(ClaimTypes.Actor, ChargeBeeSubscriptionID),
//    };
//}

#endregion
//var header = new JwtHeader(creds);
//var payload = new JwtPayload(jwtSettings.IssuerSigningKey ?? String.Empty, jwtSettings.ValidAudience ?? String.Empty, claimsList, null, DateTime.Now.AddMinutes(30));
//var token = new JwtSecurityToken(header, payload);
//return new JwtSecurityTokenHandler().WriteToken(token);
