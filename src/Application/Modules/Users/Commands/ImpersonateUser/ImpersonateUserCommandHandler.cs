using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using MediatR;

namespace Application.Modules.Users.Commands.ImpersonateUser;

public class ImpersonateUserCommandHandler : IRequestHandler<ImpersonateUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public ImpersonateUserCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) => (_unitOfWork, _currentUserService) = (unitOfWork, currentUserService);

    public async Task<bool> Handle(ImpersonateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == _currentUserService.LoggedInUser && x.IsActive && !x.IsDeleted);
        if (user is null)
        {
            throw new ClientException("No user found !", System.Net.HttpStatusCode.NotFound);
        }

        if (user.RoleIs(RoleLegend.USER))
        {
            throw new ClientException("Not allowed !", System.Net.HttpStatusCode.BadRequest);
        }

        var userImpersonation = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.Email == request.Email && x.IsActive && !x.IsDeleted);
        if (userImpersonation is null)
        {
            throw new ClientException("Invalid user for impersonation !", System.Net.HttpStatusCode.NotFound);
        }

        user.ImpersonatedAsUser = userImpersonation.ID;
        user.ImpersonatedAsRole = userImpersonation.fk_RoleID;
        _unitOfWork.Complete();

        return true;
    }


}
