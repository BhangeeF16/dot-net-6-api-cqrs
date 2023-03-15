using Utilities.Constants;

namespace Utilities.RequestRepositories.IGenericRequests
{
    public interface IGenericRequest<T> where T : class
    {
        void SetRequestCofiguration(string BaseUrl);
        void SetRequestCofiguration(string UserName, string Password, AuthenticateUsing authenticateUsing);

        Task<T> GETAsync();
        Task<TResponse> GETAsync<TResponse>();
        Task<T> GETAsync(IEnumerable<KeyValuePair<string, string>> QueryParameters);
        Task<TResponse> GETAsync<TResponse>(IEnumerable<KeyValuePair<string, string>> QueryParameters);

        Task<TResponse> POSTAsync<TResponse>(T entity);
        Task<TResponse> POSTAsync<TResponse>(T entity, IEnumerable<KeyValuePair<string, string>> QueryParameters);
        Task<TResponse> POSTAsync<TRequest, TResponse>(TRequest entity);
        Task<TResponse> POSTAsync<TRequest, TResponse>(TRequest entity, IEnumerable<KeyValuePair<string, string>> QueryParameters);

        Task<TResponse> PUTAsync<TResponse>(T entity);
        Task<TResponse> PUTAsync<TResponse>(T entity, IEnumerable<KeyValuePair<string, string>> QueryParameters);
        Task<TResponse> PUTAsync<TRequest, TResponse>(TRequest entity);
        Task<TResponse> PUTAsync<TRequest, TResponse>(TRequest entity, IEnumerable<KeyValuePair<string, string>> QueryParameters);
    }
}