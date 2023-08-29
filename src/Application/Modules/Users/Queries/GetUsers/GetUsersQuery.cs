using Application.Common.Private;
using Domain.Common.Extensions;
using Domain.Models.Pagination;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<PaginatedList<GetUsersResponse>>
{
    public int? FilterType { get; set; }
	public Pagination? Pagination { get; set; }
    public UserRoleFilter RoleFilter { get; set; } = UserRoleFilter.All;
    public GetUsersQuery(Pagination? pagination, int? roleFilter, int? filterType) => (Pagination, RoleFilter, FilterType) = (pagination, (roleFilter ?? 1).ToEnum<UserRoleFilter>(), filterType);

    public class Validator : AbstractValidator<GetUsersQuery>
    {
        public Validator() => RuleFor(c => c.RoleFilter).ValidateEnumProperty();
    }
}
