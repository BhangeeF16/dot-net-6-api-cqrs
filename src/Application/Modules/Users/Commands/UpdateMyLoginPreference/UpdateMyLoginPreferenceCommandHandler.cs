using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using MediatR;
using System.Net;

namespace Application.Modules.Users.Commands.UpdateMyLoginPreference;

public class UpdateMyLoginPreferenceCommandHandler : IRequestHandler<UpdateMyLoginPreferenceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public UpdateMyLoginPreferenceCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) => (_unitOfWork, _currentUserService) = (unitOfWork, currentUserService);

    public async Task<bool> Handle(UpdateMyLoginPreferenceCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.ID;
        var thisUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == userId && x.IsActive && !x.IsDeleted);
        if (thisUser == null)
        {
            throw new ClientException("No User Found", HttpStatusCode.NotFound);
        }
        else
        {
            if (thisUser.IsOTPLogin && !request.IsOtpLogin)
            {
                thisUser.IsOTPLogin = false;
                if (!string.IsNullOrEmpty(request.Password))
                {
                    thisUser.Password = PasswordHasher.GeneratePasswordHash(request.Password);
                }
            }

            if (!thisUser.IsOTPLogin && request.IsOtpLogin)
            {
                thisUser.IsOTPLogin = true;
            }

            _unitOfWork.Complete();
            return true;
        }
    }
}
