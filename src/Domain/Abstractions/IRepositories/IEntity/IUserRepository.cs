using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.SubscriptionModule;
using Domain.Entities.UsersModule;
using Domain.Models.Pagination;

namespace Domain.Abstractions.IRepositories.IEntity;
public interface IUserRepository : IGenericRepository<User>
{
    IGenericRepository<UserPantryItem> PantryItems { get; }
    IGenericRepository<UserLoginHistory> LoginHistories { get; }
    IGenericRepository<UserSubscription> Subscriptions { get; }
    IGenericRepository<SubscriptionSetting> SubscriptionSettings { get; }
    IGenericRepository<SubscriptionUnwantedProduce> UnwantedProduces { get; }
    IGenericRepository<SubscriptionReplacementProduce> ReplacementProduces { get; }

    Task<PaginatedList<TResponse>> GetUsersAsync<TResponse>(int? UserID, int? roleFilter, int? keywordFilter, Pagination pagination) where TResponse : class;
}
