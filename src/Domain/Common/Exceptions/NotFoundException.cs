using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Domain.Common.Exceptions;

public class NotFoundException : ClientException
{
    public NotFoundException() : base(HttpStatusCode.NotFound) { }
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.", HttpStatusCode.NotFound) { }
}