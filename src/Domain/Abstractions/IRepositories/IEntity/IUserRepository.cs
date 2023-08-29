using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.UsersModule;
using Domain.Models.Pagination;

namespace Domain.Abstractions.IRepositories.IEntity;
public interface IUserRepository : IGenericRepository<User>
{
    IGenericRepository<UserLoginHistory> LoginHistories { get; }

    Task<PaginatedList<TResponse>> GetUsersAsync<TResponse>(int? UserID, int? roleFilter, int? keywordFilter, Pagination pagination) where TResponse : class;
}
