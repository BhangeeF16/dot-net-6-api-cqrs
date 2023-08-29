using Domain.Entities.GeneralModule;
using Domain.Entities.UsersModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.LookupsModule;

[Table("Gender")]
public class Gender : AuditableEntity
{
    public Gender() : base() { }
    public Gender(string? value) : base() => Value = value;

    [Key]
    public int ID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Value { get; set; }

    public virtual ICollection<User>? Users { get; set; }
}
