using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyPassword;


public class UpdateMyPasswordCommandHandler : IRequestHandler<UpdateMyPasswordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public UpdateMyPasswordCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) => (_unitOfWork, _currentUserService) = (unitOfWork, currentUserService);

    public async Task<bool> Handle(UpdateMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.ID;
        var user = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == userId && x.IsActive && !x.IsDeleted) ?? throw new NotFoundException("No User Found !");

        if (!PasswordHasher.VerifyHash(request.OldPassword, user.Password)) throw new BadRequestException("Old password does not match");

        user.Password = PasswordHasher.GeneratePasswordHash(request.NewPassword);
        user.LastPasswordChange = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        _unitOfWork.Complete();

        return true;
    }
}
