using System.Net;

namespace Domain.Common.Exceptions;

public class BadRequestException : ClientException
{
    public BadRequestException() : base(HttpStatusCode.BadRequest) { }
    public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest) { }
    public BadRequestException(string name, object key) : base($"Entity \"{name}\" ({key}) was invalid.", HttpStatusCode.BadRequest) { }
}