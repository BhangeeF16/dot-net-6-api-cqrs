using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Constants;
using Domain.Common.Exceptions;
using MediatR;

namespace Application.Modules.Users.Commands.ResetImpersonatedUser;

public class ResetImpersonatedUserCommandHandler : IRequestHandler<ResetImpersonatedUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly ICurrentUserService _currentUserService;
    public ResetImpersonatedUserCommandHandler(IUnitOfWork unitOfWork, IChargeBeeService chargeBeeService, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _chargeBeeService = chargeBeeService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ResetImpersonatedUserCommand request, CancellationToken cancellationToken)
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

        user.ImpersonatedAsUser = null;
        user.ImpersonatedAsRole = null;
        _unitOfWork.Complete();

        return true;
    }


}
