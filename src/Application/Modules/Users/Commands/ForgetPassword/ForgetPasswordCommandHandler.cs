﻿using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using Domain.Entities.UsersModule;
using MediatR;
using Utilities.Abstractions;
using Utilities.Models;

namespace Application.Modules.Users.Commands.ForgetPassword;


public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationOptions _options;
    private readonly IEmailService _emailService;
    public ForgetPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService, ApplicationOptions options) => (_unitOfWork, _emailService, _options) = (unitOfWork, emailService, options);

    public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.Email == request.Email && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException(nameof(User), request.Email);

            var newPassword = GetRandomPassword();
            user.Password = PasswordHasher.GeneratePasswordHash(newPassword);

            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();

            #region SEND EMAIL

            var emailTask = _emailService.SendEmailAsync(new EmailOptions
            {
                Subject = $"{_options.Company} - Forget Password Request",
                IsBodyHtml = true,
                ToEmails = new List<string>() { user.Email! },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{USERNAME}}", user.FirstName! + " " + user.LastName!),
                    new KeyValuePair<string, string>("{{NEWPASSWORD}}", newPassword)
                },
                Body = FileExtensions.GetEmailTemplate(EmailTemplateLegend.ForgetPasswordEmail),
            });

            #endregion

            return true;
        }
        catch (Exception ex)
        {
            throw new BadRequestException("Some thing went wrong");
        }
    }
    private static string GetRandomPassword() => Guid.NewGuid().ToString().Replace("-", "")[..8];
}
