using API.Twilio.Service;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.IContracts.IAuth;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Domain.IContracts.IServices;
using Domain.Models.Auth;
using MediatR;

namespace Application.Modules.Users.Queries.Login;


public class LoginQueryHandler : IRequestHandler<LoginQuery, UserTokens>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITwilioService _twilioService;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public LoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator,
                             IChargeBeeService chargeBeeService,
                             IUnitOfWork unitOfWork,
                             ITwilioService twilioService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _chargeBeeService = chargeBeeService;
        _unitOfWork = unitOfWork;
        _twilioService = twilioService;
    }
    public async Task<UserTokens> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted, x => x.Subscriptions);
            if (user is null)
            {
                user ??= await _chargeBeeService.CreateUserFromChargeBeeAsync(request.Email);
                if (user == null)
                {
                    throw new ClientException("No User Found !", System.Net.HttpStatusCode.BadRequest);
                }
            }

            if (user.IsOTPLogin)
            {
                var sentOtp = user.LastLoginOTP;
                var verificationTime = Convert.ToDateTime(user.LastOtpVerification).AddMinutes(2);
                if (verificationTime < DateTime.UtcNow)
                {
                    throw new ClientException("OTP Expired !", System.Net.HttpStatusCode.BadRequest);
                }
                else if (sentOtp != Convert.ToInt32(request.Password))
                {
                    throw new ClientException("Invalid OTP !", System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    return _jwtTokenGenerator.GenerateToken(user);
                }
            }
            else
            {
                if (PasswordHasher.VerifyHash(request.Password, user.Password ?? string.Empty))
                {
                    return _jwtTokenGenerator.GenerateToken(user);
                }
                else
                {
                    throw new ClientException("Invalid Credentials !", System.Net.HttpStatusCode.BadRequest);
                }
            }
        }
        catch (Exception ex)
        {
            throw new ClientException("Some thing went wrong", System.Net.HttpStatusCode.BadRequest);
        }
    }
}
