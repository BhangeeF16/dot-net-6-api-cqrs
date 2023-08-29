using Application.Pipeline.Authentication.Basic.AuthenticationHandler;
using Application.Pipeline.Authentication.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Pipeline.Authentication.Basic;

public static class BasicAuthentication
{
    public static IServiceCollection AddBasicAuthentication(this IServiceCollection services, bool SetDefault = false)
    {
        services.AddAuthentication(options =>
        {
            if (SetDefault)
            {
                options.DefaultScheme = AuthLegend.Scheme.BASIC;
                options.DefaultAuthenticateScheme = AuthLegend.Scheme.BASIC;
                options.DefaultChallengeScheme = AuthLegend.Scheme.BASIC;
            }
        })
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthLegend.Scheme.BASIC, null);

        services.AddAuthorization(options =>
        {
            var basicPolicyBuilder = new AuthorizationPolicyBuilder(AuthLegend.Scheme.BASIC);
            basicPolicyBuilder = basicPolicyBuilder.RequireAuthenticatedUser();
            options.AddPolicy(AuthLegend.Scheme.BASIC, basicPolicyBuilder.Build());
        });

        return services;
    }
}
