using Newtonsoft.Json;
using RestSharp;

namespace Utilities.UnitOfRequests.Extensions;

public static class RestResponseExtensions
{
    public static T FormatResponse<T>(this RestResponse restResponse) where T : class
    {
        return restResponse.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(restResponse.Content) : default;
    }
    public static T FormatResponse<T>(this Task<RestResponse> restResponse) where T : class
    {
        var response = restResponse.Result;
        return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(response.Content) : default;
    }
}
