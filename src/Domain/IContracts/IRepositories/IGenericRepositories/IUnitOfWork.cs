#region Imports

using Domain.Entities.GeneralModule;
using Domain.Entities.LoggingModule;
using Domain.Entities.UsersModule;
using Domain.IContracts.IRepositories.IEntityRepositories;

#endregion

namespace Domain.IContracts.IRepositories.IGenericRepositories;

public interface IUnitOfWork : IDisposable
{
    int Complete();

    IUserRepository Users { get; }

    IGenericRepository<Role> Roles { get; }
    IGenericRepository<MiddlewareLog> MiddlewareLogs { get; }
    IGenericRepository<ApiCallLog> ApiCallLogs { get; }
    IGenericRepository<AppSetting> AppSettings { get; }
}
