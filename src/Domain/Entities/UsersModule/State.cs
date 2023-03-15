using Domain.Entities.GeneralModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.UsersModule
{
    [Table("State")]
    public class State : AuditableEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(10)]
        public string? Abbreviation { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }

}
