using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API.Private.Swagger
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerOptions(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "FarmersPick.API",
                    Description = $"{(env.IsDevelopment() ? "Development" : "Production")} Environment"
                });

                c.EnableAnnotations();

                c.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
                c.OperationFilter<SecureEndpointAuthRequirementFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme (JWT).",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddSwaggerGenNewtonsoftSupport();
            //services.AddFluentValidationRulesToSwagger();

            return services;
        }
        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app)
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
