using System.Net;

namespace Domain.ConfigurationOptions;

public class SuccessResponseModel<T> : GeneralResponseModel
{
    public T? Result { get; set; }

    public SuccessResponseModel() => (Message, StatusCode, Success) = ("Success!", HttpStatusCode.OK, true);
    public SuccessResponseModel(T result) : this() => Result = result;
    public SuccessResponseModel(T result, string message) => (Result, Message) = (result, message);
}

public class ErrorResponseModel : GeneralResponseModel
{
    public ErrorDetailResponseModel? Result { get; set; }
    public object? InternalResults { get; set; }
}

public class GeneralResponseModel
{
    public GeneralResponseModel() { }
    public GeneralResponseModel(string? message) => Message = message;

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
