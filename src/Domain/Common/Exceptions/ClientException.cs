using System.Net;

namespace Domain.Common.Exceptions
{
    public class ClientException : Exception
    {
        public bool Success { get; set; }
        public object Results { get; set; }
        public string? ExceptionMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        private ClientException() => (Success, StatusCode, Results) = (false, HttpStatusCode.BadRequest, null);

        public ClientException(string message) : this() => ExceptionMessage = message;
        public ClientException(HttpStatusCode statusCode) : this() => StatusCode = statusCode;
        public ClientException(string message, HttpStatusCode statusCode) : this() => (ExceptionMessage, StatusCode) = (message, statusCode);
        public ClientException(string message, HttpStatusCode statusCode, object results) : this()
        {
            ExceptionMessage = message;
            StatusCode = statusCode;
            Results = results;
        }
    }
}
