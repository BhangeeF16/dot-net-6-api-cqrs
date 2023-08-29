#region Imports
using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Entities.LoggingModule;
using Domain.Entities.UsersModule;
using Infrastructure.DataAccess.EntityRepositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
    private ILookupsRepository? _lookupsRepository;

    private IGenericRepository<Role>? _roleRepository;
    private IGenericRepository<ApiCallLog>? _apiCallLogsRepository;
    private IGenericRepository<AppSetting>? _appsettingsRepository;
    private IGenericRepository<MiddlewareLog>? _middlewareLogsRepository;

    #endregion

    #endregion

    #region Public Repository Creation properties...

    #region Lookups module

    public ILookupsRepository Lookups => _lookupsRepository ??= new LookupsRepository(_context, _infrastructureOptions);

    #endregion

    #region User and Role Modules

    public IUserRepository Users => _userRepository ??= new UserRepository(_context, _infrastructureOptions);
    public IGenericRepository<Role> Roles => _roleRepository ??= new GenericRepository<Role>(_context, _infrastructureOptions);

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
