using Domain.Abstractions.IRepositories.IEntity;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.ConfigurationOptions;
using Domain.Entities.RolesModule;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.EntityRepositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context, InfrastructureOptions infrastructureOptions) : base(context, infrastructureOptions) { }

    #region Inner-Repositories

    private IGenericRepository<Module>? _modules;
    public IGenericRepository<Module> Modules => _modules ??= new GenericRepository<Module>(_context, _infrastructureOptions);

    private IGenericRepository<RolePermission>? _rolePermissions;
    public IGenericRepository<RolePermission> Permissions => _rolePermissions ??= new GenericRepository<RolePermission>(_context, _infrastructureOptions);

    #endregion

    #region Methods

    public HashSet<RolePermission> GetRolePermissions() => Permissions.TableNoTracking.Include(x => x.Role).Include(x => x.Module).ToHashSet(new RolePermission.EqualityComparer());

    #endregion
}
