#region Imports
using Domain.Entities.UsersModule;
using Domain.Entities.GeneralModule;
using Domain.Entities.LoggingModule;
using Infrastructure.Persistence;
using Infrastructure.DataAccess.EntityRepositories;
using Domain.ConfigurationOptions;
using Domain.Entities.OrdersModule;
using Microsoft.EntityFrameworkCore;
using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;

#endregion

namespace Infrastructure.DataAccess.GenericRepositories;

public class UnitOfWork : IUnitOfWork
{
    #region CONSTRUCTORS AND LOCALS

    public UnitOfWork(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) => (_context, _infrastructureOptions) = (context, infrastructureOptions);    

    #region Private member variables...

    private readonly ApplicationDbContext _context;
    private readonly InfrastructureOptions _infrastructureOptions;

    private IUserRepository? _userRepository;
    private IPlanRepository? _planRepository;
    private IOrderRepository? _orderRepository;
    private ISchoolRepository? _schoolRepository;
    private IProduceRepository? _produceRepository;
    private ILookupsRepository? _lookupsRepository;
    private IPostCodeRepository? _postCodeRepository;
    private IPantryItemRepository? _pantryItemRepository;

    private IGenericRepository<Role>? _roleRepository;
    private IGenericRepository<ApiCallLog>? _apiCallLogsRepository;
    private IGenericRepository<AppSetting>? _appsettingsRepository;
    private IGenericRepository<MiddlewareLog>? _middlewareLogsRepository;
    private IGenericRepository<NonSubscribingUser>? _nonSubscribingUserRepository;

    #endregion

    #endregion

    #region Public Repository Creation properties...

    #region Lookups module

    public ILookupsRepository Lookups => _lookupsRepository ??= new LookupsRepository(_context, _infrastructureOptions);

    #endregion

    #region User and Role Modules

    public IUserRepository Users => _userRepository ??= new UserRepository(_context, _infrastructureOptions);
    public IGenericRepository<Role> Roles => _roleRepository ??= new GenericRepository<Role>(_context, _infrastructureOptions);
    public IGenericRepository<NonSubscribingUser> NonSubscribingUsers => _nonSubscribingUserRepository ??= new GenericRepository<NonSubscribingUser>(_context, _infrastructureOptions);

    #endregion

    #region Subscription and Pantry Items Modules

    public IPlanRepository Plans => _planRepository ??= new PlanRepository(_context, _infrastructureOptions);
    public IProduceRepository Produces => _produceRepository ??= new ProduceRepository(_context, _infrastructureOptions);
    public IPantryItemRepository PantryItems => _pantryItemRepository ??= new PantryItemRepository(_context, _infrastructureOptions);

    #endregion

    #region SCHOOLS MODULE

    public ISchoolRepository Schools => _schoolRepository ??= new SchoolRepository(_context, _infrastructureOptions);

    #endregion

    #region Orders Module

    public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context, _infrastructureOptions);

    #endregion

    #region Post Code Schedule Repository

    public IPostCodeRepository PostCodes => _postCodeRepository ??= new PostCodeRepository(_context, _infrastructureOptions);

    #endregion

    #region General and logging modules

    public IGenericRepository<MiddlewareLog> MiddlewareLogs => _middlewareLogsRepository ??= new GenericRepository<MiddlewareLog>(_context, _infrastructureOptions);
    public IGenericRepository<ApiCallLog> ApiCallLogs => _apiCallLogsRepository ??= new GenericRepository<ApiCallLog>(_context, _infrastructureOptions);
    public IGenericRepository<AppSetting> AppSettings => _appsettingsRepository ??= new GenericRepository<AppSetting>(_context, _infrastructureOptions);

    #endregion

    #endregion

    #region Over rides

    public void DeAttach(object Entry) => _context.Entry(Entry).State = EntityState.Detached;
    public int Complete() => _context.SaveChanges();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing) _context.Dispose();
    }

    #endregion
}
