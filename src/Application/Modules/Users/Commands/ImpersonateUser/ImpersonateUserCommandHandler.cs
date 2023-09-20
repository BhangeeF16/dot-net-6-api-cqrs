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
        var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == _currentUserService.LoggedInUser && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException("No user found !");
        
        if (user.RoleIs(RoleLegend.USER)) throw new ForbiddenAccessException("Not allowed !");

        var userImpersonation = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.Email == request.Email && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException("Invalid user for impersonation !");

        user.ImpersonatedAsUser = userImpersonation.ID;
        user.ImpersonatedAsRole = userImpersonation.fk_RoleID;
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return true;
    }


}
