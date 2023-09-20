using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using MediatR;

namespace Application.Modules.Users.Commands.ResetImpersonatedUser;

public class ResetImpersonatedUserCommandHandler : IRequestHandler<ResetImpersonatedUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public ResetImpersonatedUserCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) => (_unitOfWork, _currentUserService) = (unitOfWork, currentUserService);

    public async Task<bool> Handle(ResetImpersonatedUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == _currentUserService.LoggedInUser && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException("No user found !");

        if (user.RoleIs(RoleLegend.USER)) throw new ForbiddenAccessException("Not allowed !");

        user.ImpersonatedAsUser = null;
        user.ImpersonatedAsRole = null;
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return true;
    }


}
