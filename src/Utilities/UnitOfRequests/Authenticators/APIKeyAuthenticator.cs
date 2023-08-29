using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace Utilities.UnitOfRequests.Authenticators;

public class APIKeyAuthenticator : AuthenticatorBase
{
    private readonly string _header;
    public APIKeyAuthenticator(string APIKey, string Header = KnownHeaders.Authorization) : base(APIKey) => _header = Header;
    protected override ValueTask<Parameter> GetAuthenticationParameter(string APIKey) => new(new HeaderParameter(_header, APIKey));
}
