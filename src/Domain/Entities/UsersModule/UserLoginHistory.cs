using Domain.Entities.GeneralModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.UsersModule;

[Table("UserLoginHistory")]
public class UserLoginHistory : AuditableEntity
{
    public UserLoginHistory() : base() { }

    [Key]
    public int ID { get; set; }
    [MaxLength(50)]
    public string? UserIP { get; set; }
    [MaxLength(50)]
    public string? BrowserName { get; set; }

    [Required]
    public DateTime LoginDateTime { get; set; } = DateTime.UtcNow;
    public DateTime? LogoutDateTime { get; set; }

    [ForeignKey("User")]
    public int fk_UserID { get; set; }

    public virtual User? User { get; set; }
}
