using Application.Common.Constants;
using Domain.Common.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Application.Modules.Users.Commands.ImpersonateUser;

public class ImpersonateUserCommand : IRequest<bool>
{
    [FromHeader(Name = HeaderLegend.EMAIL)]
    public string? Email { get; set; }

    public ImpersonateUserCommand(string? email) => Email = email;
    public static ValueTask<ImpersonateUserCommand?> BindAsync(HttpContext context, ParameterInfo parameter) => ValueTask.FromResult<ImpersonateUserCommand?>(new ImpersonateUserCommand(context.Request.Headers[HeaderLegend.EMAIL]));

    public class Validator : AbstractValidator<ImpersonateUserCommand>
    {
        public Validator() => RuleFor(c => c.Email).ValidateProperty();
    }
}
