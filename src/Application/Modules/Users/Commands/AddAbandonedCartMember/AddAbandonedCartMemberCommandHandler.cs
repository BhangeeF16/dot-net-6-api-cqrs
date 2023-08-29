using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.UsersModule;
using MediatR;

namespace Application.Modules.Users.Commands.AddAbandonedCartMember;

public class AddAbandonedCartMemberCommandHandler : IRequestHandler<AddAbandonedCartMemberCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddAbandonedCartMemberCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(AddAbandonedCartMemberCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.NonSubscribingUsers.AddAsync(new NonSubscribingUser
        {
            Email = request.Email,
            PostCode = request.PostCode,
            FirstName = request.FirstName,
        });
        _unitOfWork.Complete();

        return true;
    }
}