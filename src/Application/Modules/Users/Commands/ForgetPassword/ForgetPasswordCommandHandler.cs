using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.IContracts.IRepositories.IGenericRepositories;
using MediatR;

namespace Application.Modules.Users.Commands.ForgetPassword;


public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ForgetPasswordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var DoesThisUserExist = await _unitOfWork.Users.ExistsAsync(x => x.Email == request.Email && x.IsActive == true && x.IsDeleted == false);
            if (!DoesThisUserExist)
            {
                throw new ClientException("No User Found !", System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                var newPassword = GetRandomPassword();
                var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive == true && x.IsDeleted == false, x => x.Role);
                thisUser.Password = PasswordHasher.GeneratePasswordHash(newPassword);
                _unitOfWork.Complete();

                var appSettings = await _unitOfWork.AppSettings.GetAllAsync();
                var emailTemplate = EmailUtility.GetEmailTemplateFromFile("ForgetPasswordTemplate");
                emailTemplate = emailTemplate.Replace("{{newPassword}}", newPassword);
                EmailUtility.SendMail(request.Email, emailTemplate, "Farmers Pick - Forget Password Request", emailTemplate.ToString(), appSettings.ToList());
                return true;
            }
        }
        catch (Exception ex)
        {
            throw new ClientException("Some thing went wrong", System.Net.HttpStatusCode.BadRequest);
        }
    }
    private static string GetRandomPassword()
    {
        return Guid.NewGuid().ToString().Replace("-", "")[..8];
    }
}
