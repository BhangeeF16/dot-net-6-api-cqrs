using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.RolesModule;

namespace Domain.Abstractions.IRepositories.IEntity;

public interface IRoleRepository : IGenericRepository<Role>
{
    IGenericRepository<Module> Modules { get; }
    IGenericRepository<RolePermission> Permissions { get; }

    HashSet<RolePermission> GetRolePermissions();
}
