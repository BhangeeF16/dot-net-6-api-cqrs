using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.IContracts.IAuth;
using Domain.IContracts.IRepositories.IGenericRepositories;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyPassword;


public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.OldPassword == request.ConfirmNewPassword)
            {
                throw new ClientException("Old password Cannot Be Same with New Password", System.Net.HttpStatusCode.BadRequest);
            }
            var userId = _currentUserService.ID;
            var DoesThisUserExist = await _unitOfWork.Users.ExistsAsync(x => x.ID == userId && x.IsActive == true && x.IsDeleted == false);
            if (!DoesThisUserExist)
            {
                throw new ClientException("No User Found !", System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == userId && x.IsActive == true && x.IsDeleted == false, x => x.Role);
                if (request.NewPassword == request.ConfirmNewPassword)
                {
                    if (PasswordHasher.VerifyHash(request.OldPassword, thisUser.Password))
                    {
                        thisUser.Password = PasswordHasher.GeneratePasswordHash(request.NewPassword);
                        _unitOfWork.Complete();
                        return true;
                    }
                    else
                    {
                        throw new ClientException("Old password does not match", System.Net.HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    throw new ClientException("New password does not match with Confirm Password", System.Net.HttpStatusCode.BadRequest);
                }
            }
        }
        catch (Exception ex)
        {
            throw new ClientException("Some thing went wrong", System.Net.HttpStatusCode.BadRequest);
        }
    }
}
