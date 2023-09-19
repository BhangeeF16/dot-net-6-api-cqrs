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
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.ID == _currentUserService.LoggedInUser && x.IsActive && !x.IsDeleted);
        if (user is null)
        {
            throw new ClientException("No user found !", System.Net.HttpStatusCode.NotFound);
        }

        if (user.RoleIs(RoleLegend.USER))
        {
            throw new ClientException("Not allowed !", System.Net.HttpStatusCode.BadRequest);
        }

        user.ImpersonatedAsUser = null;
        user.ImpersonatedAsRole = null;
        _unitOfWork.Complete();

        return true;
    }


}
