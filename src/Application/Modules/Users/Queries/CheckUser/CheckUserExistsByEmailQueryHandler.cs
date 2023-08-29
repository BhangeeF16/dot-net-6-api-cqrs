using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Exceptions;
using MediatR;

namespace Application.Modules.Users.Queries.CheckUser;

public class CheckUserExistsByEmailQueryHandler : IRequestHandler<CheckUserExistsByEmailQuery, CheckUserExistsByEmailQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    public CheckUserExistsByEmailQueryHandler(IUnitOfWork UnitOfWork, IChargeBeeService chargeBeeService) => (_unitOfWork, _chargeBeeService) = (UnitOfWork, chargeBeeService);

    public async Task<CheckUserExistsByEmailQueryResponse> Handle(CheckUserExistsByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()) && x.IsActive && !x.IsDeleted);
        if (user is null)
        {
            try
            {
                user ??= await _chargeBeeService.CreateUserFromChargeBeeAsync(request.Email);
            }
            catch (Exception ex)
            {
                if (ex is ClientException e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //user is null
                    }
                }
            }
        }

        if (user == null)
        {
            return new CheckUserExistsByEmailQueryResponse
            {
                DoesExist = false,
                IsPaswordLogin = null,
            };
        }
        else
        {
            return new CheckUserExistsByEmailQueryResponse
            {
                DoesExist = user != null && !string.IsNullOrEmpty(user.Email),
                IsPaswordLogin = user != null && !user.IsOTPLogin,
            };
        }
    }
}
