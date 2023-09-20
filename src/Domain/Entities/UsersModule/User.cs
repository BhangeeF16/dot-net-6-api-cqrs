using Domain.Entities.GeneralModule;
using Domain.Entities.LookupsModule;
using Domain.Entities.RolesModule;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.UsersModule;

[Table("User")]
public class User : BaseEntity
{
    public User() : base()
    {
        LoginAttempts = 0;
        IsPasswordChanged = true;
        DOB = new DateTime(1999, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc);
    }

    public User(int id, int roleID) : this() => (ID, fk_RoleID) = (id, roleID);

    [Key]
    public int ID { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(1000)]
    public string? Email { get; set; }

    [MaxLength(2000)]
    public string? Password { get; set; }

    [Required]
    [MaxLength(50)]
    public string? FirstName { get; set; }
    [Required]
    [MaxLength(50)]
    public string? LastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(450)]
    public string? Address { get; set; }

    public DateTime? DOB { get; set; }
    public DateTime? LastPasswordChange { get; set; }

    [DefaultValue(0)]
    public int LoginAttempts { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    [DefaultValue(false)]
    public bool IsPasswordChanged { get; set; } = false;

    [MaxLength(50)]
    public string? ImageKey { get; set; }

    public int? ImpersonatedAsUser { get; set; }
    public int? ImpersonatedAsRole { get; set; }

    [ForeignKey("Gender")]
    public int? fk_GenderID { get; set; }
    
    [ForeignKey("Role")]
    public int fk_RoleID { get; set; }

    public bool RoleIs(int RoleID) => this.fk_RoleID == RoleID;

    public virtual Role? Role { get; set; }
    public virtual Gender? Gender { get; set; }

    public virtual ICollection<UserLoginHistory>? LoginHistories { get; set; }
}
