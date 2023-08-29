using RestSharp;
using RestSharp.Authenticators;

namespace Utilities.UnitOfRequests.Authenticators;

public class CredentialsInHeaderAuthenticator : IAuthenticator
{
    private readonly string _token;
    private readonly string _accountID;
    public CredentialsInHeaderAuthenticator(string AccountID, string Token) => (_token, _accountID) = (Token, AccountID);

    private ValueTask<Dictionary<string, string>> GetAuthenticationParameter() => new(new Dictionary<string, string>(new KeyValuePair<string, string>[]
    {
        new KeyValuePair<string, string>("X-User-Name", _accountID),
        new KeyValuePair<string, string>("X-Password", _token),
    }));

    public async ValueTask Authenticate(IRestClient client, RestRequest request) => client.AddDefaultHeaders(await GetAuthenticationParameter().ConfigureAwait(false));
}
