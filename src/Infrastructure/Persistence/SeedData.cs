using Domain.Common.Constants;
using Domain.Common.Extensions;
using Domain.Entities.LookupsModule;
using Domain.Entities.UsersModule;

namespace Infrastructure.Persistence;

public static class SeedData
{
    private static readonly List<User> users = new()
    {
        new User
        {
            ID = 1,
            FirstName = "Application",
            LastName = "Admin",
            Email = "admin@{{DNS}}",
            Password = PasswordHasher.GeneratePasswordHash("{{GENERATE_PASSOWRD}}"),
            DOB = new DateTime(1999, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc),
            PhoneNumber = "1234567890",
            fk_GenderID = 1,
            fk_RoleID = RoleLegend.APPLICATION_ADMIN,
            LoginAttempts = 0,
            IsNewsLetter = false,
            IsOTPLogin = false,
            IsPasswordChanged = true,
            IsActive = true,
            IsDeleted = false,
        },
        new User
        {
            ID = 2,
            FirstName = "API",
            LastName = "USER",
            Email = "admin@api.com",
            Password = PasswordHasher.GeneratePasswordHash("{{GENERATE_PASSOWRD}}"),
            DOB = new DateTime(1999, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc),
            PhoneNumber = "1234567890",
            fk_GenderID = 1,
            fk_RoleID = RoleLegend.APPLICATION_ADMIN,
            LoginAttempts = 0,
            IsNewsLetter = false,
            IsOTPLogin = false,
            IsPasswordChanged = true,
            IsActive = true,
            IsDeleted = false,
        },
        new User
        {
            ID = 3,
            FirstName = "Customer",
            LastName = "Support",
            Email = "support@{{DNS}}",
            Password = PasswordHasher.GeneratePasswordHash("{{GENERATE_PASSOWRD}}"),
            DOB = new DateTime(1999, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc),
            PhoneNumber = "1234567890",
            fk_GenderID = 1,
            fk_RoleID = RoleLegend.CUSTOMER_SUPPORT,
            LoginAttempts = 0,
            IsNewsLetter = false,
            IsOTPLogin = false,
            IsPasswordChanged = true,
            IsActive = true,
            IsDeleted = false,
        },
    };
    private static readonly List<Role> roles = new()
    {
        new Role { ID = RoleLegend.APPLICATION_ADMIN, Name = "Application Admin", IsActive = true, IsDeleted = false },
        new Role { ID = RoleLegend.CUSTOMER, Name = "Customer" , IsActive = true, IsDeleted = false },
        new Role { ID = RoleLegend.CUSTOMER_SUPPORT, Name = "Customer Support" , IsActive = true, IsDeleted = false }
    };
    private static readonly List<Gender> genders = new()
    {
        new Gender { ID = 1, Value = "Male" },
        new Gender { ID = 2, Value = "FeMale" },
        new Gender { ID = 3, Value = "Other" },
    };

    public static List<User> Users { get => users; }
    public static List<Role> Roles { get => roles; }
    public static List<Gender> Genders { get => genders; }
}
