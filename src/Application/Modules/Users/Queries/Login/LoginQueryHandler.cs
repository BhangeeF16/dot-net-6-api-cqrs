using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.Models.Auth;
using MediatR;

namespace Application.Modules.Users.Queries.Login;


public class LoginQueryHandler : IRequestHandler<LoginQuery, UserTokens>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public LoginQueryHandler(IUnitOfWork unitOfWork, IChargeBeeService chargeBeeService, IJwtTokenGenerator jwtTokenGenerator)
        => (_unitOfWork, _chargeBeeService, _jwtTokenGenerator) = (unitOfWork, chargeBeeService, jwtTokenGenerator);

    public async Task<UserTokens> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _chargeBeeService.GetUserNoTrackingAsync(request.Email);
        if (user != null && user.RoleIs(RoleLegend.CUSTOMER))
        {
            await _chargeBeeService.GetUserSubscriptionNoTrackingAsync(user.ChargeBeeCustomerID, user.ID);
        }

        UserTokens userTokens = null;
        if (user.IsOTPLogin)
        {
            var sentOtp = user.LastLoginOTP;
            var verificationTime = Convert.ToDateTime(user.LastOtpVerification).AddMinutes(2);
            if (verificationTime < DateTime.UtcNow) throw new ClientException("OTP Expired", System.Net.HttpStatusCode.BadRequest);
            else if (sentOtp != Convert.ToInt32(request.Password)) throw new ClientException("Invalid OTP", System.Net.HttpStatusCode.BadRequest);
            else userTokens = _jwtTokenGenerator.GenerateToken(user);
        }
        else
        {
            if (PasswordHasher.VerifyHash(request.Password, user.Password ?? string.Empty)) userTokens = _jwtTokenGenerator.GenerateToken(user);
            else throw new ClientException("Invalid Credentials", System.Net.HttpStatusCode.BadRequest);
        }

        user.RefreshToken = userTokens.RefreshToken;
        user.RefreshTokenExpiryTime = userTokens.RefreshTokenExpiryTime;
        user.LoginAttempts++;
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return userTokens;
    }
}
