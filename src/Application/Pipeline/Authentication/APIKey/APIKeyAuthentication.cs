using Application.Pipeline.Authentication.APIKey.AuthenticationHandler;
using Application.Pipeline.Authentication.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Pipeline.Authentication.APIKey;

public static class APIKeyAuthentication
{
    public static IServiceCollection AddAPIKeyAuthentication(this IServiceCollection services, bool SetDefault = false)
    {
        services.AddAuthentication(options =>
        {
            if (SetDefault)
            {
                options.DefaultScheme = AuthLegend.Scheme.API_KEY;
                options.DefaultAuthenticateScheme = AuthLegend.Scheme.API_KEY;
                options.DefaultChallengeScheme = AuthLegend.Scheme.API_KEY;
            }
        })
        .AddScheme<AuthenticationSchemeOptions, APIKeyAuthenticationHandler>(AuthLegend.Scheme.API_KEY, null);

        services.AddAuthorization(options =>
        {
            var basicPolicyBuilder = new AuthorizationPolicyBuilder(AuthLegend.Scheme.API_KEY);
            basicPolicyBuilder = basicPolicyBuilder.RequireAuthenticatedUser();
            options.AddPolicy(AuthLegend.Scheme.API_KEY, basicPolicyBuilder.Build());
        });

        return services;
    }
}
