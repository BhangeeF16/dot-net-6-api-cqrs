using Domain.Common.Constants;
using Domain.Common.Extensions;
using Domain.Entities.GeneralModule;
using Domain.Entities.LookupsModule;
using Domain.Entities.RolesModule;
using Domain.Entities.UsersModule;

namespace Infrastructure.Persistence;

public static class SeedData
{
    private static readonly List<User> _users = new()
    {
        new User(UserLegend.APPLICATION_ADMIN, RoleLegend.APPLICATION_ADMIN)
        {
            FirstName = "Application",
            LastName = "Admin",
            Email = "admin@{{DNS}}",
            Password = PasswordHasher.GeneratePasswordHash("{{GENERATE_PASSOWRD}}"),
            PhoneNumber = "1234567890",
            fk_GenderID = GenderLegend.MALE,
            fk_RoleID = RoleLegend.APPLICATION_ADMIN,
        },
        new User(UserLegend.API_ADMIN, RoleLegend.APPLICATION_ADMIN)
        {
            FirstName = "API",
            LastName = "USER",
            Email = "admin@api.com",
            Password = PasswordHasher.GeneratePasswordHash("{{GENERATE_PASSOWRD}}"),
            PhoneNumber = "1234567890",
            fk_GenderID = GenderLegend.MALE,
        },
        new User(UserLegend.SUPPORT, RoleLegend.SUPPORT)
        {
            FirstName = "Customer",
            LastName = "Support",
            Email = "support@{{DNS}}",
            Password = PasswordHasher.GeneratePasswordHash("{{GENERATE_PASSOWRD}}"),
            PhoneNumber = "1234567890",
            fk_GenderID = GenderLegend.MALE,
        },
    };
    private static readonly List<Role> _roles = new()
    {
        new Role(RoleLegend.APPLICATION_ADMIN, "Application Admin"),
        new Role(RoleLegend.USER, "User"),
        new Role(RoleLegend.SUPPORT, "User Support")
    };
    private static readonly List<Gender> _genders = new()
    {
        new Gender(GenderLegend.MALE, "Male"),
        new Gender(GenderLegend.FEMALE, "Female"),
        new Gender(GenderLegend.OTHER, "Other"),
    };
    private static readonly List<Module> _modules = new()
    {
        new Module(ModuleLegend.USERS, ModuleLegend.USERS_MODULE),
        new Module(ModuleLegend.ROLES, ModuleLegend.ROLES_MODULE),
    };
    private static readonly List<RolePermission> _rolePermissions = new()
    {
        new RolePermission()
        { 
            ID = 1, PermissionLevel = PermissionLevel.All,
            fk_RoleID = RoleLegend.APPLICATION_ADMIN, fk_ModuleID = ModuleLegend.USERS,
        },
        new RolePermission()
        { 
            ID = 2, PermissionLevel = PermissionLevel.All,
            fk_RoleID = RoleLegend.APPLICATION_ADMIN, fk_ModuleID = ModuleLegend.ROLES, 
        },
        new RolePermission()
        {
            ID = 3, PermissionLevel = PermissionLevel.None,
            fk_RoleID = RoleLegend.USER, fk_ModuleID = ModuleLegend.USERS,
        },
        new RolePermission()
        {
            ID = 4, PermissionLevel = PermissionLevel.None,
            fk_RoleID = RoleLegend.USER, fk_ModuleID = ModuleLegend.ROLES,
        },
    };

    public static List<Role> Roles => _roles;
    public static List<User> Users => _users;
    public static List<Gender> Genders => _genders;
    public static List<Module> Modules => _modules;
    public static List<RolePermission> RolePermissions => _rolePermissions;
}
