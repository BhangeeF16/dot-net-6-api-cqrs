using Domain.Entities.LoggingModule;
using Newtonsoft.Json;
using Utilities.Constants;
using Utilities.RequestRepositories.IGenericRequests;

namespace Utilities.RequestRepositories.GenericRequests
{
    public class GenericRequest<T> : IGenericRequest<T> where T : class
    {
        #region Private member variables...

        private readonly RequestConfiguration _config;
        private readonly IRequestHelper _requestHelper;
        private readonly IApiRequestLogger? _apiRequestLogger;
        public GenericRequest(IApiRequestLogger apiRequestLogger, IRequestHelper requestHelper, RequestConfiguration requestCofiguration)
        {
            _requestHelper = requestHelper;
            _config = requestCofiguration;
            _apiRequestLogger = apiRequestLogger;
        }
        

        #endregion

        public void SetRequestCofiguration(string BaseUrl) => _config.BaseUrl = BaseUrl;
        public void SetRequestCofiguration(string UserName, string Password, AuthenticateUsing authenticateUsing)
        {
            _config.UserName = UserName;
            _config.Token = Password;
            _config.AuthenticateUsing = authenticateUsing;
        }

        public async Task<T> GETAsync()
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "GET",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };

            return await _apiRequestLogger.LogApiRequestsAsync<T>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessGetRequestByBasicAuth(_config.URL, _config.UserName, _config.Token),
                AuthenticateUsing.Bearer => _requestHelper.ProcessGetRequestByBearer(_config.URL, _config.Token),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessGetRequestByHeaderAuth(_config.URL, _config.UserName, _config.Token),
                _ => _requestHelper.ProcessGetRequest(_config.URL, null),
            });
        }
        public async Task<TResponse> GETAsync<TResponse>()
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "GET",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessGetRequestByBasicAuth(_config.URL, _config.UserName, _config.Token),
                AuthenticateUsing.Bearer => _requestHelper.ProcessGetRequestByBearer(_config.URL, _config.Token),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessGetRequestByHeaderAuth(_config.URL, _config.UserName, _config.Token),
                _ => _requestHelper.ProcessGetRequest(_config.URL, null),
            });
        }
        public async Task<T> GETAsync(IEnumerable<KeyValuePair<string, string>> QueryParameters)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "GET",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<T>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessGetRequestByBasicAuth(_config.URL, _config.UserName, _config.Token, QueryParameters.ToArray()),
                AuthenticateUsing.Bearer => _requestHelper.ProcessGetRequestByBearer(_config.URL, _config.Token, QueryParameters.ToArray()),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessGetRequestByHeaderAuth(_config.URL, _config.UserName, _config.Token, QueryParameters.ToArray()),
                _ => _requestHelper.ProcessGetRequest(_config.URL, QueryParameters.ToArray()),
            });
        }
        public async Task<TResponse> GETAsync<TResponse>(IEnumerable<KeyValuePair<string, string>> QueryParameters)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "GET",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessGetRequestByBasicAuth(_config.URL, _config.UserName, _config.Token, QueryParameters.ToArray()),
                AuthenticateUsing.Bearer => _requestHelper.ProcessGetRequestByBearer(_config.URL, _config.Token, QueryParameters.ToArray()),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessGetRequestByHeaderAuth(_config.URL, _config.UserName, _config.Token, QueryParameters.ToArray()),
                _ => _requestHelper.ProcessGetRequest(_config.URL, QueryParameters.ToArray()),
            });
        }

        public async Task<TResponse> POSTAsync<TResponse>(T entity)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "POST",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPostRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPostRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPostRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                _ => _requestHelper.ProcessPostRequest(entity, _config.URL, _config.UseAccept),
            });
        }
        public async Task<TResponse> POSTAsync<TResponse>(T entity, IEnumerable<KeyValuePair<string, string>> QueryParameters)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "POST",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPostRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPostRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPostRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                _ => _requestHelper.ProcessPostRequest(entity, _config.URL, _config.UseAccept, QueryParameters.ToArray()),
            });
        }
        public async Task<TResponse> POSTAsync<TRequest, TResponse>(TRequest entity)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "POST",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPostRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPostRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPostRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                _ => _requestHelper.ProcessPostRequest(entity, _config.URL, _config.UseAccept),
            });
        }
        public async Task<TResponse> POSTAsync<TRequest, TResponse>(TRequest entity, IEnumerable<KeyValuePair<string, string>> QueryParameters)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "POST",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPostRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPostRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPostRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                _ => _requestHelper.ProcessPostRequest(entity, _config.URL, _config.UseAccept, QueryParameters.ToArray()),
            });
        }

        public async Task<TResponse> PUTAsync<TResponse>(T entity)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "PUT",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPutRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPutRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPutRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                _ => _requestHelper.ProcessPutRequest(entity, _config.URL, _config.UseAccept),
            });
        }
        public async Task<TResponse> PUTAsync<TResponse>(T entity, IEnumerable<KeyValuePair<string, string>> QueryParameters)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "PUT",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPutRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPutRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPutRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                _ => _requestHelper.ProcessPutRequest(entity, _config.URL, _config.UseAccept, QueryParameters.ToArray()),
            });
        }
        public async Task<TResponse> PUTAsync<TRequest, TResponse>(TRequest entity)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "PUT",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPutRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPutRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPutRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept),
                _ => _requestHelper.ProcessPutRequest(entity, _config.URL, _config.UseAccept),
            });
        }
        public async Task<TResponse> PUTAsync<TRequest, TResponse>(TRequest entity, IEnumerable<KeyValuePair<string, string>> QueryParameters)
        {
            var apiCallLog = new ApiCallLog
            {
                RequestMethod = "PUT",
                RequestUrl = _config.BaseUrl,
                EndPoint = _config.Endpoint,
                RequestBody = entity is string ? entity.ToString() : JsonConvert.SerializeObject(entity),
                StartDateTime = DateTime.UtcNow,
                IsSuccessfull = true
            };
            return await _apiRequestLogger.LogApiRequestsAsync<TResponse>(apiCallLog, () => _config.AuthenticateUsing switch
            {
                AuthenticateUsing.BasicAuth => _requestHelper.ProcessPutRequestByBasicAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.Bearer => _requestHelper.ProcessPutRequestByBearer(entity, _config.URL, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                AuthenticateUsing.CredentialsInHeaders => _requestHelper.ProcessPutRequestByHeaderAuth(entity, _config.URL, _config.UserName, _config.Token, _config.UseAccept, QueryParameters.ToArray()),
                _ => _requestHelper.ProcessPutRequest(entity, _config.URL, _config.UseAccept, QueryParameters.ToArray()),
            });
        }
    }
}
