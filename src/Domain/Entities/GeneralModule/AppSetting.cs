#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.GeneralModule;

[Table("AppSetting")]
public class AppSetting : BaseEntity
{
    public AppSetting() : base() { }

    [Key]
    public int ID { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [MaxLength(50)]
    public string Value { get; set; }
    [Required]
    [MaxLength(50)]
    public string Label { get; set; }
    [MaxLength(2000)]
    public string Description { get; set; }
}
