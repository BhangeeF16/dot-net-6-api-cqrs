using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Common.Constants;
using Domain.ConfigurationOptions;
using Domain.Entities.SubscriptionModule;
using Domain.Entities.UsersModule;
using Domain.Models.Pagination;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace Infrastructure.DataAccess.EntityRepositories;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) : base(context, infrastructureOptions) { }

    #region Inner-Repositories

    private IGenericRepository<UserPantryItem>? _userPantryItems;
    private IGenericRepository<UserSubscription>? _userSubscriptions;
    private IGenericRepository<UserLoginHistory>? _userLoginHistories;
    private IGenericRepository<SubscriptionSetting>? _subscriptionSettingRepository;
    private IGenericRepository<SubscriptionUnwantedProduce>? _subscriptionUnwantedProduceRepository;
    private IGenericRepository<SubscriptionReplacementProduce>? _subscriptionReplacementProduceRepository;

    public IGenericRepository<UserLoginHistory> LoginHistories => _userLoginHistories ??= new GenericRepository<UserLoginHistory>(_context, _infrastructureOptions);
    public IGenericRepository<UserSubscription> Subscriptions => _userSubscriptions ??= new GenericRepository<UserSubscription>(_context, _infrastructureOptions);
    public IGenericRepository<SubscriptionSetting> SubscriptionSettings => _subscriptionSettingRepository ??= new GenericRepository<SubscriptionSetting>(_context, _infrastructureOptions);
    public IGenericRepository<SubscriptionUnwantedProduce> UnwantedProduces => _subscriptionUnwantedProduceRepository ??= new GenericRepository<SubscriptionUnwantedProduce>(_context, _infrastructureOptions);
    public IGenericRepository<SubscriptionReplacementProduce> ReplacementProduces => _subscriptionReplacementProduceRepository ??= new GenericRepository<SubscriptionReplacementProduce>(_context, _infrastructureOptions);
    public IGenericRepository<UserPantryItem> PantryItems => _userPantryItems ??= new GenericRepository<UserPantryItem>(_context, _infrastructureOptions);

    #endregion

    #region Methods


    public async Task<PaginatedList<TResponse>> GetUsersAsync<TResponse>(int? UserID, int? roleFilter, int? keywordFilter, Pagination pagination) where TResponse : class
    {
        return await ExecuteSqlStoredProcedureAsync<TResponse>(StoredProceduresLegend.GetUsers, pagination, new List<SqlParameter>()
        {
            new SqlParameter("@userID", UserID <= 0 ? DBNull.Value : UserID),
            new SqlParameter("@roleFilter", roleFilter <= 0 ? DBNull.Value : roleFilter),
            new SqlParameter("@keywordFilter", keywordFilter <= 0 ? DBNull.Value : keywordFilter),

        });
    }

    #endregion
}
