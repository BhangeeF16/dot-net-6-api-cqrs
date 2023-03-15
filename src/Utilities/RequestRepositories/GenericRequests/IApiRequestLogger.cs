using Domain.Entities.LoggingModule;
using RestSharp;

namespace Utilities.RequestRepositories.GenericRequests
{
    public interface IApiRequestLogger
    {
        Task<T> LogApiRequestsAsync<T>(ApiCallLog callLog, Func<RestResponse> function);
    }
}