using Domain.ConfigurationOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Private.MinimalModule;

public class Header : FromHeaderAttribute
{
    public Header(string name) : base() => Name = name;
}

public static class MinimalAttributeExtensions
{
    public static RouteHandlerBuilder AllowAnonymous(this RouteHandlerBuilder endpoint)
    {
        endpoint.WithMetadata(new AllowAnonymousAttribute());
        return endpoint;
    }
    public static RouteHandlerBuilder Authorize(this RouteHandlerBuilder endpoint, string policy = null, string[] roles = null, params string[] schemes)
    {
        endpoint.WithMetadata(new AuthorizeAttribute()
        {
            Policy = policy,
            Roles = (roles != null && roles.Any()) ? string.Join(',', roles) : null,
            AuthenticationSchemes = (schemes != null && schemes.Any()) ? string.Join(',', schemes) : null
        });

        return endpoint;
    }
    public static RouteHandlerBuilder AddMetaData<T>(this RouteHandlerBuilder endpoint, string summary = null, string description = null)
    {
        endpoint.WithMetadata(new SwaggerOperationAttribute(summary, description));

        endpoint.WithMetadata(new SwaggerResponseAttribute(200, type: typeof(SuccessResponseModel<T>)))
                .WithMetadata(new SwaggerResponseAttribute(500, type: typeof(ProblemDetails)))
                .WithMetadata(new SwaggerResponseAttribute(400, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(404, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(422, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(304, type: typeof(ErrorResponseModel)));

        return endpoint;
    }
    public static RouteHandlerBuilder AddMetaData<T>(this RouteHandlerBuilder endpoint, string tag = null, string summary = null, string description = null)
    {
        endpoint.WithTags(tag);

        endpoint.WithMetadata(new SwaggerOperationAttribute(summary, description));

        endpoint.WithMetadata(new SwaggerResponseAttribute(200, type: typeof(SuccessResponseModel<T>)))
                .WithMetadata(new SwaggerResponseAttribute(500, type: typeof(ProblemDetails)))
                .WithMetadata(new SwaggerResponseAttribute(400, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(404, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(422, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(304, type: typeof(ErrorResponseModel)));

        return endpoint;
    }
    public static RouteHandlerBuilder AddMetaData(this RouteHandlerBuilder endpoint, string tag, string summary = null, string description = null)
    {
        endpoint.WithTags(tag);

        endpoint.WithMetadata(new SwaggerOperationAttribute(summary, description));

        endpoint.WithMetadata(new SwaggerResponseAttribute(500, type: typeof(ProblemDetails)))
                .WithMetadata(new SwaggerResponseAttribute(400, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(404, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(422, type: typeof(ErrorResponseModel)))
                .WithMetadata(new SwaggerResponseAttribute(304, type: typeof(ErrorResponseModel)));

        return endpoint;
    }
}
