using Microsoft.AspNetCore.Http;

namespace Application.Pipeline.Middlewares.RequestTimeOut;

public class RequestTimeOutMiddleware
{
    private readonly RequestDelegate _next;
    public RequestTimeOutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    //you can not inject it as a constructor parameter in Middleware because only Singleton services can be resolved by constructor injection in Middleware.
    public async Task Invoke(HttpContext context)
    {
        var requestAborted = context.RequestAborted;
        var timeoutDuration = TimeSpan.FromMinutes(30); // Set the desired timeout duration

        context.Request.HttpContext.Response.RegisterForDispose(requestAborted.Register(() =>
        {
            // Handle the timeout or cancellation logic
        }));

        // Set the timeout duration
        context.RequestAborted = new CancellationTokenSource(timeoutDuration).Token;

        await Task.CompletedTask;
    }
}