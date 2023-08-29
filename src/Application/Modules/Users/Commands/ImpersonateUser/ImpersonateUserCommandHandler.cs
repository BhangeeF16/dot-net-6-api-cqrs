using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using MediatR;

namespace Application.Modules.Users.Commands.ImpersonateUser;

public class ImpersonateUserCommandHandler : IRequestHandler<ImpersonateUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ICurrentUserService _currentUserService;
    public ImpersonateUserCommandHandler(IUnitOfWork unitOfWork, IChargeBeeService chargeBeeService, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _chargeBeeService = chargeBeeService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ImpersonateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == _currentUserService.LoggedInUser && x.IsActive && !x.IsDeleted);
        if (user is null)
        {
            throw new ClientException("No user found !", System.Net.HttpStatusCode.NotFound);
        }

        if (user.RoleIs(RoleLegend.CUSTOMER))
        {
            throw new ClientException("Not allowed !", System.Net.HttpStatusCode.BadRequest);
        }

        var userImpersonation = await _chargeBeeService.GetUserNoTrackingAsync(request.Email);
        if (userImpersonation is null)
        {
            throw new ClientException("Invalid user for impersonation !", System.Net.HttpStatusCode.NotFound);
        }

        if (userImpersonation.RoleIs(RoleLegend.CUSTOMER))
        {
            await _chargeBeeService.GetUserSubscriptionNoTrackingAsync(userImpersonation.ChargeBeeCustomerID, userImpersonation.ID);
        }

        user.ImpersonatedAsUser = userImpersonation.ID;
        user.ImpersonatedAsRole = userImpersonation.fk_RoleID;
        _unitOfWork.Complete();

        return true;
    }


}
