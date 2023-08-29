using Newtonsoft.Json;
using RestSharp;
using Utilities.UnitOfRequests.Models;

namespace Utilities.UnitOfRequests.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest GetRequest(this RequestConfiguration configuration, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => Request(configuration, Method.Get, null, QueryParameters, Headers);

    public static RestRequest PutRequest(this RequestConfiguration configuration, object Body, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => Request(configuration, Method.Put, Body, QueryParameters, Headers);

    public static RestRequest PostRequest(this RequestConfiguration configuration, object Body, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) 
        => Request(configuration, Method.Post, Body, QueryParameters, Headers);

    public static RestRequest PatchRequest(this RequestConfiguration configuration, object Body, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null) 
        => Request(configuration, Method.Patch, Body, QueryParameters, Headers);

    private static RestRequest Request(this RequestConfiguration configuration, Method method, object Body = null, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
    {
        var request = new RestRequest(configuration.URL, method)
        {
            Timeout = int.MaxValue
        };

        if (QueryParameters != null && QueryParameters.Any())
        {
            for (int i = 0; i < QueryParameters.Length; i++)
            {
                request.AddQueryParameter(QueryParameters[i].Key, QueryParameters[i].Value);
            }
        }

        if (Headers != null && Headers.Any())
        {
            for (int i = 0; i < Headers.Length; i++)
            {
                request.AddHeader(Headers[i].Key, Headers[i].Value);
            }
        }

        if (configuration.UseAccept)
        {
            request.AddHeader("Accept", "application/json");
        }

        if (configuration.BodyParameterType == BodyParameterType.FormUrlEncoded || configuration.BodyParameterType == BodyParameterType.FormData)
        {
            if (Body is not null)
            {
                var bodyParams = (KeyValuePair<string, string>[])Body;
                if (bodyParams != null && bodyParams.Any())
                {
                    for (int i = 0; i < bodyParams.Length; i++)
                    {
                        request.AddParameter(bodyParams[i].Key, bodyParams[i].Value);
                    }
                }
            }
        }

        switch (configuration.BodyParameterType)
        {
            case BodyParameterType.FormData:
                request.AlwaysMultipartFormData = true;
                break;
            case BodyParameterType.FormUrlEncoded:
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                break;
            case BodyParameterType.XML:
                request.AddHeader("Content-Type", "text/xml");
                request.AddXmlBody(Body, "application/xml");
                break;
            case BodyParameterType.JSON:
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(Body, "application/json");
                break;
            case BodyParameterType.None:
            default:
                break;
        }

        return request;
    }
}
