using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.ConfigurationOptions;
using Domain.Entities.LookupsModule;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;

namespace Infrastructure.DataAccess.EntityRepositories;

public class LookupsRepository : ILookupsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly InfrastructureOptions _infrastructureOptions;
    public LookupsRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) => (_context, _infrastructureOptions) = (context, infrastructureOptions);

    #region Inner-Repositories

    private IGenericRepository<Gender>? _genders;
    public IGenericRepository<Gender> Genders => _genders ??= new GenericRepository<Gender>(_context, _infrastructureOptions);

    #endregion

    #region Methods

    #endregion
}
