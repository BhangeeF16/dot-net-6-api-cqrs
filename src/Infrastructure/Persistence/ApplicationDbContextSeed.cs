using Domain.Entities.GeneralModule;
using Domain.Entities.LookupsModule;
using Domain.Entities.PlansModule;
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

        modelBuilder.Entity<Day>().HasData(SeedData.Days);
        modelBuilder.Entity<Carrier>().HasData(SeedData.Carriers);
        modelBuilder.Entity<TimeSlot>().HasData(SeedData.TimeSlots);
        modelBuilder.Entity<AppSetting>().HasData(SeedData.AppSettings);

        modelBuilder.Entity<Plan>().HasData(SeedData.Plans);
        modelBuilder.Entity<PlanVariation>().HasData(SeedData.PlanVariations);

        modelBuilder.Entity<User>().HasData(SeedData.Users);
    }
}
