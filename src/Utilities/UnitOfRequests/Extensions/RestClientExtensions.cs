using RestSharp;
using RestSharp.Authenticators;
using Utilities.UnitOfRequests.Authenticators;
using Utilities.UnitOfRequests.Models;

namespace Utilities.UnitOfRequests.Extensions;

public static class RestClientExtensions
{
    public static RestClientOptions AddAuthentication(this RestClientOptions options, RequestConfiguration configuration)
    {
        switch (configuration.AuthenticateUsing)
        {
            case AuthenticateUsing.BasicAuth:
                options.Authenticator = new HttpBasicAuthenticator(configuration.AccountID, configuration.Token);
                break;
            case AuthenticateUsing.JWT:
                options.Authenticator = new JwtAuthenticator(configuration.Token);
                break;
            case AuthenticateUsing.CredentialsInHeaders:
                options.Authenticator = new CredentialsInHeaderAuthenticator(configuration.AccountID, configuration.Token);
                break;
            case AuthenticateUsing.APIKey:
                options.Authenticator = new APIKeyAuthenticator(configuration.Token, configuration.Header);
                break;
            case AuthenticateUsing.None:
            default:
                break;
        }

        return options;
    }
    public static RestResponse ProcessGetRequest(this RestClient client, RequestConfiguration configuration, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => client.ExecuteAsync(configuration.GetRequest(QueryParameters, Headers)).GetAwaiter().GetResult();
    public static RestResponse ProcessPutRequest(this RestClient client, RequestConfiguration configuration, object body, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => client.ExecuteAsync(configuration.PutRequest(body, QueryParameters, Headers)).GetAwaiter().GetResult();
    public static RestResponse ProcessPostRequest(this RestClient client, RequestConfiguration configuration, object body, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => client.ExecuteAsync(configuration.PostRequest(body, QueryParameters, Headers)).GetAwaiter().GetResult();
    public static RestResponse ProcessPatchRequest(this RestClient client, RequestConfiguration configuration, object body, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        => client.ExecuteAsync(configuration.PatchRequest(body, QueryParameters, Headers)).GetAwaiter().GetResult();
}