using Application.Pipeline.Authentication.Extensions;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Models.Auth;
using MediatR;
using System.Security.Claims;

namespace Application.Modules.Users.Queries.RefreshToken;


public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, UserTokens>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentUserService _currentUserService;
    public RefreshTokenQueryHandler(IUnitOfWork unitOfWork,
                                    IJwtTokenGenerator jwtTokenGenerator,
                                    ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentUserService = currentUserService;
    }
    public async Task<UserTokens> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        string? refreshToken = request.RefreshToken;
        var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(_currentUserService.AccessToken.ExtractToken());
        if (principal == null)
        {
            throw new ClientException("Invalid access token or refresh token", System.Net.HttpStatusCode.BadRequest);
        }

        var claims = principal.Identities.First()?.Claims?.ToList() ?? new List<Claim>();
        string email = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value.ToLower() ?? string.Empty;

        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()) && x.IsActive && !x.IsDeleted);

        if (string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiryTime == null || !user.RefreshTokenExpiryTime.HasValue)
        {
            throw new ClientException("No Tokens found, Please try logging out of the system and log in again", System.Net.HttpStatusCode.BadRequest);
        }

        if (user.RefreshToken != refreshToken)
        {
            throw new ClientException("Invalid refresh token", System.Net.HttpStatusCode.BadRequest);
        }

        if (!(user.RefreshTokenExpiryTime.Value.CompareTo(DateTime.UtcNow) >= 0))
        {
            throw new ClientException("Refresh token is expired, Please try logging out of the system and log in again", System.Net.HttpStatusCode.BadRequest);
        }

        var userTokens = _jwtTokenGenerator.GenerateToken(user);

        user.RefreshToken = userTokens.RefreshToken;
        user.RefreshTokenExpiryTime = userTokens.RefreshTokenExpiryTime;
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return userTokens;
    }
}
