using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Domain.Entities.LoggingModule;

[Table("MiddlewareLog")]
public class MiddlewareLog
{
    public MiddlewareLog() { }

    public int ID { get; set; }
    [Required]
    [MaxLength(1000)]
    public string RequestURL { get; set; }
    [Required]
    [MaxLength(50)]
    public string IPAddress { get; set; }
    [Required]
    [MaxLength(450)]
    public string RequestByURL { get; set; }
    public string RequestBody { get; set; }
    public string Response { get; set; }
    public int ResponseStatusCode { get; set; }
    public DateTime RequestAt { get; set; }
    public DateTime? ResponseAt { get; set; }
}
