using Application.Common.Extensions;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Utilities.Abstractions;
using Utilities.Models;

namespace Application.Modules.Users.Queries.SendOtpForLogin;

public class SendOtpForLoginQueryHandler : IRequestHandler<SendOtpForLoginQuery, bool>
{
    #region CONSTRUCTORS AND LOCALS

    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationOptions _options;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _hostEnvironment;
    public SendOtpForLoginQueryHandler(IUnitOfWork UnitOfWork,
                                       ApplicationOptions options,
                                       IEmailService emailService,
                                       IWebHostEnvironment hostEnvironment)
    {
        _options = options;
        _unitOfWork = UnitOfWork;
        _emailService = emailService;
        _hostEnvironment = hostEnvironment;
    }

    #endregion

    public async Task<bool> Handle(SendOtpForLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted);

        if (user.IsOTPLogin)
        {
            user.PhoneNumber = user.PhoneNumber.FormatPhoneNumber();
            var OTP = _hostEnvironment.IsDevelopment() ? "0000" : new Random().Next(1000, 9999).ToString();

            #region TEXT / SMS OTP

            if (request.Via == Common.Private.SendOtpVia.Text)
            {
                _twilioService.SendSMS(OTP, user.PhoneNumber);
            }

            #endregion

            #region EMAIL OTP

            if (request.Via == Common.Private.SendOtpVia.Email)
            {
                _emailService.SendEmailAsync(new EmailOptions
                {
                    Subject = $"{_options.Company} - OTP Code",
                    IsBodyHtml = true,
                    ToEmails = new List<string>()
                    {
                        user.Email!
                    },
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{USERNAME}}", user.FirstName! + " " + user.LastName!),
                        new KeyValuePair<string, string>("{{OTPCODE}}", OTP.ToString())
                    },
                    Body = FileExtensions.GetEmailTemplate(EmailTemplateLegend.OtpEmail),
                });
            }

            #endregion

            #region DB UPDATES

            user.LastLoginOTP = Convert.ToInt32(OTP);
            user.LastOtpVerification = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();

            #endregion

            return true;
        }
        else
        {
            throw new ClientException("Invalid Login method !", System.Net.HttpStatusCode.BadRequest);
        }
    }
}
