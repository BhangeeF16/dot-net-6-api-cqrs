﻿using API.Twilio.Service;
using Application.Common.Extensions;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
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
    private readonly IEmailService _emailService;
    private readonly ITwilioService _twilioService;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly IWebHostEnvironment _hostEnvironment;
    public SendOtpForLoginQueryHandler(IUnitOfWork UnitOfWork,
                                       IEmailService emailService,
                                       ITwilioService twilioService,
                                       IChargeBeeService chargeBeeService,
                                       IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = UnitOfWork;
        _emailService = emailService;
        _twilioService = twilioService;
        _hostEnvironment = hostEnvironment;
        _chargeBeeService = chargeBeeService;
    }
    
    #endregion

    public async Task<bool> Handle(SendOtpForLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _chargeBeeService.GetUserNoTrackingAsync(request.Email);

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
                    Subject = "Farmers Pick - OTP Code",
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
