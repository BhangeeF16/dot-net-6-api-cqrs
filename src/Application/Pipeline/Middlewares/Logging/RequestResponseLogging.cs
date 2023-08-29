using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.LoggingModule;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace Application.Pipeline.Middlewares.Logging;

public class RequestResponseLogging
{
    private readonly RequestDelegate _next;
    public RequestResponseLogging(RequestDelegate next) => _next = next;

    //you can not inject it as a constructor parameter in Middleware because only Singleton services can be resolved by constructor injection in Middleware.
    public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
    {
        var apiLog = await FormatRequest(context);
        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next.Invoke(context);

                apiLog.Response = await FormatResponse(context.Response);
                apiLog.ResponseStatusCode = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                apiLog.Response = JsonConvert.SerializeObject(ex);
                apiLog.ResponseStatusCode = context.Response.StatusCode;
            }

            apiLog.ResponseAt = DateTime.Now;

            if (
                (apiLog.Response.IndexOf("healthy", StringComparison.InvariantCultureIgnoreCase) < 0)
                &&
                (apiLog.RequestURL.IndexOf("swagger", StringComparison.InvariantCultureIgnoreCase) < 0 && apiLog.RequestByURL.IndexOf("swagger", StringComparison.InvariantCultureIgnoreCase) < 0)
                &&
                (apiLog.RequestURL.IndexOf("hangfire", StringComparison.InvariantCultureIgnoreCase) < 0 && apiLog.RequestByURL.IndexOf("hangfire", StringComparison.InvariantCultureIgnoreCase) < 0)
               )
            {
                await unitOfWork.MiddlewareLogs.AddAsync(apiLog);
                unitOfWork.Complete();
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private static async Task<MiddlewareLog> FormatRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];

        await context.Request.Body.ReadAsync(buffer);

        var bodyAsText = Encoding.UTF8.GetString(buffer);
        context.Request.Body.Position = 0;

        var logDto = new MiddlewareLog
        {
            RequestAt = DateTime.Now,
            RequestURL = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path} {context.Request.QueryString}".Trim(),
            RequestByURL = context.Request.Headers["Referer"].ToString(),
            IPAddress = context.Connection.RemoteIpAddress?.ToString(),
            RequestBody = bodyAsText
        };
        context.Items["RequestLog"] = logDto;
        return logDto;
    }

    private static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);

        string text = await new StreamReader(response.Body).ReadToEndAsync();

        response.Body.Seek(0, SeekOrigin.Begin);

        return text;
    }
}

