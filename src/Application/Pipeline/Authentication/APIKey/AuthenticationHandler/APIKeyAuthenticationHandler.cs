using Application.Common.Constants;
using Application.Pipeline.Authentication.Extensions;
using Domain.Common.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Application.Pipeline.Authentication.APIKey.AuthenticationHandler;
public class APIKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public APIKeyAuthenticationHandler(ISystemClock clock,
                                       UrlEncoder encoder,
                                       ILoggerFactory logger,
                                       IConfiguration configuration,
                                       IHttpContextAccessor httpContextAccessor,
                                       IOptionsMonitor<AuthenticationSchemeOptions> options) : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var IsApiKeyPresent = _httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue(HeaderLegend.API_KEY, out var extractedApiKey);
            if (IsApiKeyPresent.HasValue && IsApiKeyPresent.Value && !string.IsNullOrEmpty(extractedApiKey))
            {
                if (_configuration.GetAPIKeys().Contains(extractedApiKey))
                {
                    var claimsList = new List<Claim>();
                    var claimsIdentity = new ClaimsIdentity(claimsList, AuthLegend.Scheme.API_KEY);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    var authTicket = new AuthenticationTicket(claimsPrincipal, AuthLegend.Scheme.API_KEY);

                    return await Task.FromResult(AuthenticateResult.Success(authTicket));
                }
                else
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Invalid Credentials"));
                }
            }
            else
            {
                return await Task.FromResult(AuthenticateResult.Fail("Missing credentials"));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail(ex));
        }
    }
}
