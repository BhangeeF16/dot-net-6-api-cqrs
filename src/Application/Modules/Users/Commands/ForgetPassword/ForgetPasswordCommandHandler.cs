using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using MediatR;
using Utilities.Abstractions;
using Utilities.Models;
using Utilities.Services;
using static System.Net.WebRequestMethods;

namespace Application.Modules.Users.Commands.ForgetPassword;


public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    public ForgetPasswordCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService) => (_unitOfWork, _emailService) = (unitOfWork, emailService);

    public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.Email == request.Email && x.IsActive && !x.IsDeleted);
            if (user is null)
            {
                throw new ClientException("No User Found !", System.Net.HttpStatusCode.BadRequest);
            }

            var newPassword = GetRandomPassword();
            user.Password = PasswordHasher.GeneratePasswordHash(newPassword);

            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();

            #region SEND EMAIL

            _emailService.SendEmailAsync(new EmailOptions
            {
                Subject = "Farmers Pick - Forget Password Request",
                IsBodyHtml = true,
                ToEmails = new List<string>()
                {
                    user.Email!
                },
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
            throw new ClientException("Some thing went wrong", System.Net.HttpStatusCode.BadRequest);
        }
    }
    private static string GetRandomPassword() => Guid.NewGuid().ToString().Replace("-", "")[..8];
}
