#region Imports

using Domain.Abstractions.IRepositories.IEntity;
using Domain.Entities.GeneralModule;
using Domain.Entities.LoggingModule;
using Domain.Entities.UsersModule;

#endregion

namespace Domain.Abstractions.IRepositories.IGeneric;

public interface IUnitOfWork : IDisposable
{
    int Complete();
    void DeAttach(object Entry);

    IUserRepository Users { get; }
    ILookupsRepository Lookups { get; }


    IGenericRepository<Role> Roles { get; }
    IGenericRepository<ApiCallLog> ApiCallLogs { get; }
    IGenericRepository<AppSetting> AppSettings { get; }
    IGenericRepository<MiddlewareLog> MiddlewareLogs { get; }
}
