using Domain.Abstractions.IRepositories.IGeneric;
using MediatR;

namespace Application.Modules.Users.Queries.CheckUser;

public class CheckUserExistsByEmailQueryHandler : IRequestHandler<CheckUserExistsByEmailQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public CheckUserExistsByEmailQueryHandler(IUnitOfWork UnitOfWork) => _unitOfWork = UnitOfWork;

    public async Task<bool> Handle(CheckUserExistsByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Users.ExistsAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted);
    }
}
