using System.Net;

namespace Domain.Common.Exceptions;

public class ForbiddenAccessException : ClientException
{
    public ForbiddenAccessException() : base(HttpStatusCode.Forbidden) { }
    public ForbiddenAccessException(string message) : base(message, HttpStatusCode.Forbidden) { }
}