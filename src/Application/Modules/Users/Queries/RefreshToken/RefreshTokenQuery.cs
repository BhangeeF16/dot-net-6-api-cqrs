using Application.Common.Constants;
using Domain.Common.Extensions;
using Domain.Models.Auth;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Application.Modules.Users.Queries.RefreshToken;

public class RefreshTokenQuery : IRequest<UserTokens>
{
    public RefreshTokenQuery(string? token) => RefreshToken = token;

    [FromHeader(Name = HeaderLegend.REFRESH_TOKEN)]
    public string? RefreshToken { get; set; }

    public static ValueTask<RefreshTokenQuery?> BindAsync(HttpContext context, ParameterInfo parameter) => ValueTask.FromResult<RefreshTokenQuery?>(new RefreshTokenQuery(context.Request.Headers[HeaderLegend.REFRESH_TOKEN]));
    public class Validator : AbstractValidator<RefreshTokenQuery>
    {
        public Validator() => RuleFor(c => c.RefreshToken).ValidateProperty();
    }
}
