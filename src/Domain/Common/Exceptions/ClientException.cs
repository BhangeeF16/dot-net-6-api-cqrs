using System.Net;

namespace Domain.Common.Exceptions
{
    public class ClientException : Exception
    {
        public string? ExceptionMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public object Results { get; set; }
        public bool Success { get; set; }
        public ClientException(string message)
        {
            Success = false;
            ExceptionMessage = message;
            StatusCode = HttpStatusCode.BadRequest;
            Results = null;

        }
        public ClientException(string message, HttpStatusCode statusCode, object Results)
        {
            Success = false;
            ExceptionMessage = message;
            StatusCode = statusCode;
            this.Results = Results;

        }
        public ClientException(string message, HttpStatusCode statusCode)
        {
            Success = false;
            ExceptionMessage = message;
            StatusCode = statusCode;
            Results = null;

        }
    }
}
