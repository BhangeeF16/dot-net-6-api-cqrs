#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.LoggingModule;

[Table("ApiCallLog")]
public class ApiCallLog
{
    public ApiCallLog() { }

    public int ID { get; set; }
    [MaxLength(50)]
    public string EndPoint { get; set; }
    [MaxLength(450)]
    public string Notes { get; set; }
    [MaxLength(1000)]
    public string RequestUrl { get; set; }
    [MaxLength(450)]
    public string RequestMethod { get; set; }
    public string RequestBody { get; set; }
    public string Response { get; set; }
    public int ResponseStatusCode { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public bool IsSuccessfull { get; set; }
    public bool IsException { get; set; }
    public string ExceptionMessage { get; set; }
}
