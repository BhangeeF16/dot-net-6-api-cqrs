using Domain.Entities.GeneralModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.RolesModule;

[Table("Module")]
public class Module : BaseEntity
{
    public Module() : base() { }
    public Module(string name) : base() => Name = name;
    public Module(int iD, string name) : base() => (ID, Name) = (iD, name);

    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }
    public virtual ICollection<RolePermission>? Permissions { get; set; }
}
