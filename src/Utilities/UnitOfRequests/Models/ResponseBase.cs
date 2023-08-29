namespace Utilities.UnitOfRequests.Models;

public class ResponseBase
{
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccessful { get; set; }
}
