using System.Net;

namespace Domain.ConfigurationOptions;
// Success Response model
public class SuccessResponseModel<T> : GeneralResponseModel
{
    public T? Result { get; set; }
}
// Error Response model
public class ErrorResponseModel : GeneralResponseModel
{
    public ErrorDetailResponseModel? Result { get; set; }
    public object? InternalResults { get; set; }
}
// Every Response inclueded the response
public class GeneralResponseModel
{
    public string? Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool Success { get; set; }
}
// Used for Exception Response
public class ErrorDetailResponseModel
{
    public string? StackTrace { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? ExceptionMessageDetail { get; set; }
    public string? ReferenceErrorCode { get; set; }
    public object? ValidationErrors { get; set; }
}
