using Domain.Abstractions.IRepositories.IGeneric;
using MediatR;

namespace Application.Modules.Users.Queries.CheckUser;

public class CheckUserExistsByEmailQueryHandler : IRequestHandler<CheckUserExistsByEmailQuery, CheckUserExistsByEmailQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public CheckUserExistsByEmailQueryHandler(IUnitOfWork UnitOfWork) => _unitOfWork = UnitOfWork;

    public async Task<CheckUserExistsByEmailQueryResponse> Handle(CheckUserExistsByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted);
        return user == null
            ? new CheckUserExistsByEmailQueryResponse(false, null)
            : new CheckUserExistsByEmailQueryResponse(!string.IsNullOrEmpty(user.Email), !user.IsOTPLogin);
    }
}
