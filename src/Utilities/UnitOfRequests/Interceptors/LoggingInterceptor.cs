using Azure.Core;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.LoggingModule;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Utilities.UnitOfRequests.Interceptors;
public class LoggingInterceptor : DelegatingHandler, IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    public LoggingInterceptor(ILogger<LoggingInterceptor> logger, IUnitOfWork unitOfWork) : base(new HttpClientHandler())
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;
        ApiCallLog apiCallLog = null;

        try
        {
            apiCallLog = LogRequest(request);
            response = base.Send(request, cancellationToken);
            LogResponse(response, ref apiCallLog);
        }
        catch (Exception ex)
        {
            apiCallLog.IsException = true;
            apiCallLog.ExceptionMessage = ex.Message;
            apiCallLog.IsSuccessfull = false;
        }
        finally
        {
            apiCallLog.EndDateTime = DateTime.UtcNow;
            _unitOfWork.ApiCallLogs.AddAsync(apiCallLog).GetAwaiter().GetResult();
            _unitOfWork.Complete();
        }

        return response;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;
        ApiCallLog apiCallLog = null;

        try
        {
            apiCallLog = LogRequest(request);
            response = await base.SendAsync(request, cancellationToken);
            LogResponse(response, ref apiCallLog);
        }
        catch (Exception ex)
        {
            apiCallLog.IsException = true;
            apiCallLog.ExceptionMessage = ex.Message;
            apiCallLog.IsSuccessfull = false;
            _logger.LogError(ex.Message);
        }
        finally
        {
            apiCallLog.EndDateTime = DateTime.UtcNow;
            await _unitOfWork.ApiCallLogs.AddAsync(apiCallLog);
            _unitOfWork.Complete();
        }

        return response;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // dispose others
        }
    }
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await Task.CompletedTask;

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private ApiCallLog LogRequest(HttpRequestMessage request)
    {
        var apiCallLog = new ApiCallLog
        {
            RequestMethod = request.Method.Method.ToUpper(),
            RequestUrl = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}{request.RequestUri.PathAndQuery}".Trim(),
            EndPoint = request.RequestUri.PathAndQuery,
            StartDateTime = DateTime.UtcNow,
            IsSuccessfull = true
        };

        apiCallLog.EndPoint = apiCallLog.EndPoint.Length >= 50 ? apiCallLog.EndPoint[..50] : apiCallLog.EndPoint;
        apiCallLog.RequestUrl = apiCallLog.RequestUrl.Length >= 1000 ? apiCallLog.RequestUrl[..1000] : apiCallLog.RequestUrl;

        var info = new StringBuilder().AppendFormat("Outgoing Request:\nURL: {0}\nMethod: {1}", apiCallLog.RequestUrl, apiCallLog.RequestMethod);

        if (request.Headers != null && request.Headers.Any())
        {
            info.AppendLine("\nHeaders:");
            info.AppendLine(string.Join("\n\t", request.Headers.Select(x => $"{x.Key} : {string.Join(",", x.Value)}")));
        }

        if (request.Content != null)
        {
            apiCallLog.RequestBody = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            info.AppendLine("\nContent:");
            info.AppendLine(apiCallLog.RequestBody.Length >= 450 ? "Content character length is exceeding thus it is hidden" : apiCallLog.RequestBody);
        }

        info.AppendLine();

        _logger.LogInformation(info.ToString());

        return apiCallLog;
    }
    private void LogResponse(HttpResponseMessage response, ref ApiCallLog apiCallLog)
    {
        var info = new StringBuilder().AppendFormat("Response:\nStatus Code: {0}", response.StatusCode);

        if (response.Headers != null && response.Headers.Any())
        {
            info.AppendLine("\nHeaders:");
            info.AppendLine(string.Join("\n\t", response.Headers.Select(x => $"{x.Key} : {string.Join(",", x.Value)}")));
        }

        if (response.Content != null)
        {
            apiCallLog.Response = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            info.AppendLine("\nContent:");
            info.AppendLine(apiCallLog.Response.Length >= 450 ? "Content character length is exceeding thus it is hidden" : apiCallLog.Response);
        }

        info.AppendLine();

        _logger.LogInformation(info.ToString());

        apiCallLog.ResponseStatusCode = (int)response.StatusCode;
        response.EnsureSuccessStatusCode();
        apiCallLog.IsSuccessfull = response.IsSuccessStatusCode;
    }
}
