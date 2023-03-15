using Domain.Common.Constants;
using Domain.ConfigurationOptions;
using Domain.Entities.SubscriptionModule;
using Domain.Entities.UsersModule;
using Domain.IContracts.IRepositories.IEntityRepositories;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Domain.Models.Pagination;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace Infrastructure.DataAccess.EntityRepositories;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly InfrastructureOptions _connectionInfo;

    public UserRepository(ApplicationDbContext dbContext, InfrastructureOptions connectionInfo) : base(dbContext, connectionInfo)
    {
        _dbContext = dbContext;
        _connectionInfo = connectionInfo;
    }

    #region Inner-Repositories

    private IGenericRepository<UserLoginHistory>? _userLoginHistories;
    private IGenericRepository<UserSubscription>? _userSubscriptions;
    private IGenericRepository<UserPantryItem>? _userPantryItems;
    private IGenericRepository<SubscriptionSetting>? _subscriptionSettingRepository;
    private IGenericRepository<Country>? _countries;
    private IGenericRepository<State>? _states;
    private IGenericRepository<Gender>? _genders;

    public IGenericRepository<UserLoginHistory> LoginHistories
    {
        get
        {
            if (_userLoginHistories == null)
                _userLoginHistories = new GenericRepository<UserLoginHistory>(_dbContext, _connectionInfo);
            return _userLoginHistories;
        }
    }
    public IGenericRepository<UserSubscription> Subscriptions
    {
        get
        {
            if (_userSubscriptions == null)
                _userSubscriptions = new GenericRepository<UserSubscription>(_dbContext, _connectionInfo);
            return _userSubscriptions;
        }
    }
    public IGenericRepository<SubscriptionSetting> SubscriptionSettings
    {
        get
        {
            if (_subscriptionSettingRepository == null)
                _subscriptionSettingRepository = new GenericRepository<SubscriptionSetting>(_dbContext, _connectionInfo);
            return _subscriptionSettingRepository;
        }
    }
    public IGenericRepository<UserPantryItem> PantryItems
    {
        get
        {
            if (_userPantryItems == null)
                _userPantryItems = new GenericRepository<UserPantryItem>(_dbContext, _connectionInfo);
            return _userPantryItems;
        }
    }
    public IGenericRepository<Country> Countries
    {

        get
        {
            if (_countries == null)
                _countries = new GenericRepository<Country>(_dbContext, _connectionInfo);
            return _countries;
        }
    }
    public IGenericRepository<State> States
    {

        get
        {
            if (_states == null)
                _states = new GenericRepository<State>(_dbContext, _connectionInfo);
            return _states;
        }
    }
    public IGenericRepository<Gender> Genders
    {

        get
        {
            if (_genders == null)
                _genders = new GenericRepository<Gender>(_dbContext, _connectionInfo);
            return _genders;
        }
    }

    #endregion

    #region Methods

    #endregion
}
