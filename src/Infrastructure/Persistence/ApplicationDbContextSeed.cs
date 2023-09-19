using Domain.Entities.LookupsModule;
using Domain.Entities.RolesModule;
using Domain.Entities.UsersModule;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{

    public static void SeedSampleDataAsync(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gender>().HasData(SeedData.Genders);
        modelBuilder.Entity<Role>().HasData(SeedData.Roles);
        modelBuilder.Entity<User>().HasData(SeedData.Users);
        modelBuilder.Entity<Module>().HasData(SeedData.Modules);
        modelBuilder.Entity<RolePermission>().HasData(SeedData.RolePermissions);
    }
}
