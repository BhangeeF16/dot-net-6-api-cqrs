using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Models.Pagination;
using MediatR;

namespace Application.Modules.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<GetUsersResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public GetUsersQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) => (_unitOfWork, _currentUserService) = (unitOfWork, currentUserService);

    public async Task<PaginatedList<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Users.GetUsersAsync<GetUsersResponse>(_currentUserService.ID, ((int)request.RoleFilter), request.FilterType, (request.Pagination ?? new Pagination()));
    }
}
