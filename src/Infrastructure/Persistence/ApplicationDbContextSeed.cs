using Domain.Entities.GeneralModule;
using Domain.Entities.UsersModule;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{

    public static void SeedSampleDataAsync(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(SeedData.Roles);
        modelBuilder.Entity<Gender>().HasData(SeedData.Genders);
        modelBuilder.Entity<Country>().HasData(SeedData.Countries);
        modelBuilder.Entity<State>().HasData(SeedData.States);
        modelBuilder.Entity<User>().HasData(SeedData.User);
        modelBuilder.Entity<AppSetting>().HasData(SeedData.AppSettings);
    }
}
