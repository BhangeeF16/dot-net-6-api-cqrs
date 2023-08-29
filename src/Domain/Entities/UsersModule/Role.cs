using Domain.Entities.GeneralModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.UsersModule;

[Table("Role")]
public class Role : BaseEntity
{
    public Role() : base() { }
    public Role(string name) : base() => Name = name;

    [Key]
    public int ID { get; set; }
    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    public virtual ICollection<User>? Users { get; set; }
}
