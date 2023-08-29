using Application.Pipeline.Middlewares.Logging;
using Microsoft.AspNetCore.Builder;

namespace Application.Pipeline.Middlewares;

public static class Dependencyinjection
{
    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestResponseLogging>();
        //app.UseMiddleware<RequestTimeOutMiddleware>();

        return app;
    }
}
