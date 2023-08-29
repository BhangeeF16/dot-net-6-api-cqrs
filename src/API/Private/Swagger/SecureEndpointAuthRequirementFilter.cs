using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Private.Swagger
{
    public class SecureEndpointAuthRequirementFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
            {
                return;
            }

            var authorize = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().FirstOrDefault();

            if (authorize == null || string.IsNullOrEmpty(authorize.AuthenticationSchemes))
            {
                return;
            }

            var schemes = authorize.AuthenticationSchemes.Split(',').Distinct().Select(x => x.Trim()).ToList();
            var OpenApiSecurityRequirements = schemes.Select(x => new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = x
                    },
                    In = ParameterLocation.Header,
                }] = new List<string>(),
            }
            ).ToList();

            operation.Security = OpenApiSecurityRequirements;
        }
    }
}
