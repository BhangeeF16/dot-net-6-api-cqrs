using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.IContracts.IRepositories.IEntityRepositories;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;

namespace Infrastructure.DataAccess.EntityRepositories;
public class AppSettingsRepository : GenericRepository<AppSetting>, IAppSettingsRepository
{
    private readonly ApplicationDbContext _dbContext;
    public AppSettingsRepository(ApplicationDbContext dbContext, InfrastructureOptions connectionInfo) : base(dbContext, connectionInfo)
    {
        _dbContext = dbContext;
    }
    public Task<string> UpsertAppSettings(AppSetting appSetting)
    {
        if (appSetting.ID == 0)
        {
            var createData = AddAsync(appSetting);
            return Task.FromResult("User is Successfully saved");
        }
        else
        {
            Update(appSetting);
            return Task.FromResult("User is Updated Successfully ");
        }
    }
}
