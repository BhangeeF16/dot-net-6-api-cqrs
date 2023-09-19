using Domain.Entities.GeneralModule;
using Domain.Entities.UsersModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.RolesModule;

[Table("Role")]
public class Role : BaseEntity
{
    public Role() : base() { }
    public Role(string name) : base() => Name = name;
    public Role(int iD, string name) : base() => (ID, Name) = (iD, name);

    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    public virtual ICollection<User>? Users { get; set; }
    public virtual ICollection<RolePermission>? Permissions { get; set; }
}
