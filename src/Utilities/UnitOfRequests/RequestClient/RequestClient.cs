using RestSharp;
using Utilities.Abstractions;
using Utilities.UnitOfRequests.Extensions;
using Utilities.UnitOfRequests.Interceptors;
using Utilities.UnitOfRequests.Models;

namespace Utilities.UnitOfRequests.RequestClient;
public class RequestClient<T> : IRequestClient<T> where T : class
{
    #region Private member variables...

    private readonly RestClient _client;
    private readonly RequestConfiguration _configuration;
    public RequestClient(RequestConfiguration configuration, LoggingInterceptor loggingInterceptor = null)
    {
        _configuration = configuration;

        var options = new RestClientOptions
        {
            BaseUrl = new Uri(_configuration.BaseUrl),
            MaxTimeout = int.MaxValue,
        };

        if (loggingInterceptor is not null)
        {
            options.ConfigureMessageHandler = _ => loggingInterceptor;
        }

        options.AddAuthentication(_configuration);

        _client = new RestClient(options);
    }

    #endregion

    #region manage configuration

    public void SetEndPoint(string EndPoint) => _configuration.Endpoint = EndPoint;
    public void SetAccessToken(string AccessToken) => _configuration.Token = AccessToken;
    public void SetBaseUrl(string BaseUrl) => _configuration.BaseUrl = BaseUrl;
    public void SetRequestCofiguration(string UserName, string Password, AuthenticateUsing authenticateUsing)
    {
        _configuration.AccountID = UserName;
        _configuration.Token = Password;
        _configuration.AuthenticateUsing = authenticateUsing;
    }

    #endregion

    #region METHODS

    public T GET(KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => _client.ProcessGetRequest(_configuration, QueryParameters, Headers).FormatResponse<T>();
    public TResponse GET<TResponse>(KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class
        => _client.ProcessGetRequest(_configuration, QueryParameters, Headers).FormatResponse<TResponse>();

    public TResponse POST<TResponse>(T entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class
        => _client.ProcessPostRequest(_configuration, entity, QueryParameters, Headers).FormatResponse<TResponse>();
    public TResponse POST<TRequest, TResponse>(TRequest entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class
        => _client.ProcessPostRequest(_configuration, entity, QueryParameters, Headers).FormatResponse<TResponse>();

    public TResponse PUT<TResponse>(T entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class
        => _client.ProcessPutRequest(_configuration, entity, QueryParameters, Headers).FormatResponse<TResponse>();
    public TResponse PUT<TRequest, TResponse>(TRequest entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class
        => _client.ProcessPutRequest(_configuration, entity, QueryParameters, Headers).FormatResponse<TResponse>();

    #endregion
}
