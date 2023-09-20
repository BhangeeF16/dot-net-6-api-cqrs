using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.Entities.UsersModule;
using Domain.Models.Auth;
using MediatR;

namespace Application.Modules.Users.Queries.Login;


public class LoginQueryHandler : IRequestHandler<LoginQuery, UserTokens>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public LoginQueryHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator) => (_unitOfWork, _jwtTokenGenerator) = (unitOfWork, jwtTokenGenerator);

    public async Task<UserTokens> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException(nameof(User), request.Email);

        if (!PasswordHasher.VerifyHash(request.Password, user.Password ?? string.Empty)) throw new BadRequestException("Invalid Credentials");

        var userTokens = _jwtTokenGenerator.GenerateToken(user);
        user.RefreshToken = userTokens.RefreshToken;
        user.RefreshTokenExpiryTime = userTokens.RefreshTokenExpiryTime;
        user.LoginAttempts++;
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return userTokens;
    }
}
