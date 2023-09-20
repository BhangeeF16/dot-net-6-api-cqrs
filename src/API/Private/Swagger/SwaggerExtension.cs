using Application.Common.Constants;
using Application.Pipeline.Authentication.Extensions;
using Domain.Common.Extensions;
using Domain.ConfigurationOptions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API.Private.Swagger
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerOptions(this IServiceCollection services, IWebHostEnvironment env)
        {
            var options = services.GetService<ApplicationOptions>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"{options.Company}.API",
                    Description = $"{(env.IsDevelopment() ? "Development" : env.IsProduction() ? "Production" : "Staging")} Environment"
                });

                c.EnableAnnotations();

                c.OperationFilter<SecurityRequirementsOperationFilter>(true, AuthLegend.Scheme.API_KEY);
                c.OperationFilter<SecurityRequirementsOperationFilter>(true, AuthLegend.Scheme.BEARER);
                c.OperationFilter<SecurityRequirementsOperationFilter>(true, AuthLegend.Scheme.BASIC);
                c.OperationFilter<SecureEndpointAuthRequirementFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.AddSecurityDefinition(AuthLegend.Scheme.BEARER, new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme (JWT).",
                    Name = HeaderLegend.AUTHORIZATION,
                    Scheme = AuthLegend.Scheme.BEARER,
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                });

                c.AddSecurityDefinition(AuthLegend.Scheme.API_KEY, new OpenApiSecurityScheme()
                {
                    Description = "Authorization by x-api-key inside request's header",
                    Name = HeaderLegend.API_KEY,
                    Scheme = AuthLegend.Scheme.API_KEY,
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                });

                c.AddSecurityDefinition(AuthLegend.Scheme.BASIC, new OpenApiSecurityScheme
                {
                    Description = "Basic Authorization header using the Basic scheme",
                    Name = HeaderLegend.AUTHORIZATION,
                    Scheme = AuthLegend.Scheme.BASIC,
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }
        public static IApplicationBuilder UseSwaggerOptions(this IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.DefaultModelExpandDepth(2);
                c.DefaultModelsExpandDepth(-1);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.None);
                c.EnableDeepLinking();
                c.ShowExtensions();
                c.ShowCommonExtensions();
                c.EnableValidator();
                c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Put, SubmitMethod.Post, SubmitMethod.Delete);
                c.UseRequestInterceptor("(request) => { return request; }");
                c.UseResponseInterceptor("(response) => { return response; }");
            });

            return app;
        }
        public static IEndpointRouteBuilder MapSwaggerDoc(this IEndpointRouteBuilder route)
        {
            route.MapSwagger();

            return route;
        }
    }
}
