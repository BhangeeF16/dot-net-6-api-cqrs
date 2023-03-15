using Domain.Common.Exceptions;
using Domain.IContracts.IRepositories.IGenericRepositories;
using MediatR;

namespace Application.Modules.Users.Queries.SendOtpForLogin;

public class SendOtpForLoginQueryHandler : IRequestHandler<SendOtpForLoginQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITwilioService _twilioService;
    private readonly IChargeBeeService _chargeBeeService;
    public SendOtpForLoginQueryHandler(IUnitOfWork UnitOfWork, ITwilioService twilioService, IChargeBeeService chargeBeeService)
    {
        _unitOfWork = UnitOfWork;
        _twilioService = twilioService;
        _chargeBeeService = chargeBeeService;
    }
    public async Task<bool> Handle(SendOtpForLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted);
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
            var OTP = new Random().Next(1000, 9999);
            _twilioService.SendSMS(OTP.ToString(), user.PhoneNumber);
            user.LastLoginOTP = OTP;
            user.LastOtpVerification = DateTime.UtcNow;
            _unitOfWork.Complete();
            return true;
        }
        else
        {
            throw new ClientException("Invalid Login method !", System.Net.HttpStatusCode.BadRequest);
        }
    }
}
