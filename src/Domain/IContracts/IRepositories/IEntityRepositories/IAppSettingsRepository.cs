using Domain.Entities.GeneralModule;
using Domain.IContracts.IRepositories.IGenericRepositories;

namespace Domain.IContracts.IRepositories.IEntityRepositories;

public interface IAppSettingsRepository : IGenericRepository<AppSetting>
{
    Task<string> UpsertAppSettings(AppSetting appSetting);
}
