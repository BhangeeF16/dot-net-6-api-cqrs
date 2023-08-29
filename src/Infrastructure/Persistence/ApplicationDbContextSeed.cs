using Domain.Entities.LookupsModule;
using Domain.Entities.UsersModule;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{

    public static void SeedSampleDataAsync(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(SeedData.Roles);
        modelBuilder.Entity<Gender>().HasData(SeedData.Genders);
        modelBuilder.Entity<User>().HasData(SeedData.Users);
    }
}
