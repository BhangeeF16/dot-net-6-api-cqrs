using Utilities.UnitOfRequests.Models;

namespace Utilities.Abstractions
{
    public interface IRequestClient<T> where T : class
    {
        void SetBaseUrl(string BaseUrl);
        void SetEndPoint(string EndPoint);
        void SetAccessToken(string AccessToken);
        void SetRequestCofiguration(string UserName, string Password, AuthenticateUsing authenticateUsing);

        T GET(KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        TResponse GET<TResponse>(KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class;
        TResponse POST<TResponse>(T entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class;
        TResponse POST<TRequest, TResponse>(TRequest entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class;
        TResponse PUT<TResponse>(T entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class;
        TResponse PUT<TRequest, TResponse>(TRequest entity, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) where TResponse : class;

    }
}