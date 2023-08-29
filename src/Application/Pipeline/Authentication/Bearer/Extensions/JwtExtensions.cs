using Application.Common.Constants;
using Application.Pipeline.Authentication.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Application.Pipeline.Authentication.Bearer.Extensions;

public static class JwtExtensions
{
    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    public static JwtBearerEvents SetJwtEvents(this JwtBearerEvents authOptions)
    {
        authOptions = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authorization = context?.Request?.Headers[HeaderLegend.AUTHORIZATION].ToString() ?? string.Empty;

                // If no authorization header found, nothing to process further
                if (string.IsNullOrEmpty(authorization))
                {
                    context?.NoResult();
                    return Task.CompletedTask;
                }
                context.Token = authorization.ExtractToken();

                // If no token found, no further work possible
                if (string.IsNullOrEmpty(context.Token))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    context.Response.Headers.Add("Token-Expired", "true");

                context.NoResult();
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.NoResult();
                return Task.CompletedTask;
            }
        };
        return authOptions;
    }
}