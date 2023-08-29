using Application.Pipeline.Authentication.Bearer.Extensions;
using Application.Pipeline.Authentication.Extensions;
using Domain.Abstractions.IAuth;
using Domain.ConfigurationOptions;
using Domain.Entities.UsersModule;
using Domain.Models.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Pipeline.Authentication.Bearer.TokenGenerator;
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IDatetimeProvider _dateTimeProvider;
    private readonly JwtSettings _jwtSettings;
    public JwtTokenGenerator(IDatetimeProvider dateTimeProvider, IOptions<JwtSettings> jwtSettings)
    {
        _dateTimeProvider = dateTimeProvider;
        _jwtSettings = jwtSettings.Value;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, GetValidationParameters(), out SecurityToken securityToken);

        if (
                securityToken is not JwtSecurityToken jwtSecurityToken
                ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
            )
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public UserTokens GenerateToken(User user)
    {
        try
        {
            var claimsList = user.GetClaims();

            // using symmetricKey because we later need to authenticateit (not using IDENTITY SERVER)

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresIn = _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryAfterMinutes);

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                expires: expiresIn,
                claims: claimsList,
                signingCredentials: creds);

            return new UserTokens()
            {
                AccessTokenExpiryTime = expiresIn,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken),
                RefreshToken = JwtExtensions.GenerateRefreshToken(),
                RefreshTokenExpiryTime = expiresIn.AddMinutes(30),
            };
        }
        catch (Exception)
        {

            throw;
        }
    }
    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey)),
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidIssuer = _jwtSettings.ValidIssuer,
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidAudience = _jwtSettings.ValidAudience,
            RequireExpirationTime = _jwtSettings.RequireExpirationTime,
            ValidateLifetime = _jwtSettings.RequireExpirationTime,
            ClockSkew = TimeSpan.FromDays(1),
        };
    }
}

//public UserTokens GenerateToken(User user, UserSubscription userSubscription)
    //{
    //    try
    //    {
    //        var claimsList = user.GetClaims(userSubscription.ChargeBeeID);

    //        // using symmetricKey because we later need to authenticateit (not using IDENTITY SERVER)

    //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey ?? string.Empty));
    //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //        var expiresIn = _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryAfterMinutes);

    //        var securityToken = new JwtSecurityToken(
    //            issuer: _jwtSettings.ValidIssuer,
    //            audience: _jwtSettings.ValidAudience,
    //            expires: expiresIn,
    //            claims: claimsList,
    //            signingCredentials: creds);

    //        return new UserTokens()
    //        {
    //            //UserId = user.ID,
    //            //FullName = $"{user.FirstName} {user.LastName}",
    //            //Email = user.Email,
    //            //UserName = user.Email,
    //            //RoleId = user.fk_RoleID,
    //            AccessTokenExpiryTime = expiresIn,
    //            AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken),
    //            RefreshToken = JwtExtensions.GenerateRefreshToken(),
    //            RefreshTokenExpiryTime = expiresIn.AddMinutes(30),
    //        };
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}