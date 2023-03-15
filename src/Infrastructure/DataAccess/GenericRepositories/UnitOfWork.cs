#region Imports
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Entities.LoggingModule;
using Domain.Entities.UsersModule;
using Domain.IContracts.IRepositories.IEntityRepositories;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Infrastructure.DataAccess.EntityRepositories;
using Infrastructure.Persistence;

#endregion

namespace Infrastructure.DataAccess.GenericRepositories;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext appDbContext, InfrastructureOptions connectionInfo)
    {
        _context = appDbContext;
        _connectionInfo = connectionInfo;
    }

    #region Private member variables...

    private readonly ApplicationDbContext _context;
    private readonly InfrastructureOptions _connectionInfo;

    private IUserRepository? _userRepository;

    private IGenericRepository<Role>? _roleRepository;
    private IGenericRepository<MiddlewareLog>? _middlewareLogsRepository;
    private IGenericRepository<ApiCallLog>? _apiCallLogsRepository;
    private IGenericRepository<AppSetting>? _appsettingsRepository;

    #endregion

    #region Public Repository Creation properties...

    #region User and Role Modules

    public IUserRepository Users
    {
        get
        {
            if (_userRepository == null)
                _userRepository = new UserRepository(_context, _connectionInfo);
            return _userRepository;
        }
    }
    public IGenericRepository<Role> Roles
    {
        get
        {
            if (_roleRepository == null)
                _roleRepository = new GenericRepository<Role>(_context, _connectionInfo);
            return _roleRepository;
        }
    }

    #endregion

    #region General and logging modules

    public IGenericRepository<MiddlewareLog> MiddlewareLogs
    {
        get
        {
            if (_middlewareLogsRepository == null)
                _middlewareLogsRepository = new GenericRepository<MiddlewareLog>(_context, _connectionInfo);
            return _middlewareLogsRepository;
        }
    }
    public IGenericRepository<ApiCallLog> ApiCallLogs
    {
        get
        {
            if (_apiCallLogsRepository == null)
                _apiCallLogsRepository = new GenericRepository<ApiCallLog>(_context, _connectionInfo);
            return _apiCallLogsRepository;
        }
    }
    public IGenericRepository<AppSetting> AppSettings
    {
        get
        {
            if (_appsettingsRepository == null)
                _appsettingsRepository = new GenericRepository<AppSetting>(_context, _connectionInfo);
            return _appsettingsRepository;
        }
    }

    #endregion

    #endregion

    #region Over rides

    public int Complete()
    {
        return _context.SaveChanges();
    }
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
