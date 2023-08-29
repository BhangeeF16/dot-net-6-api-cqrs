using Application.Pipeline.Authentication.Extensions;
using Application.Pipeline.Authorization.Requirements.IsAllowed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Application.Pipeline.Authorization;

public static class DependencyInjection
{
    public static IServiceCollection AddPermissionRequirements(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, IsAllowedRequirementHandler>();

        return services;
    }

    public static IServiceCollection AddRoleAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthLegend.Policy.APPLICATION_ADMIN_ONLY,
                policyBuilder => policyBuilder.RequireClaim(ClaimTypes.PrimaryGroupSid, AuthLegend.Role.APPLICATION_ADMIN));

            options.AddPolicy(
                AuthLegend.Policy.ADMINS_ONLY,
                policyBuilder => policyBuilder.RequireClaim(ClaimTypes.PrimaryGroupSid, AuthLegend.Role.APPLICATION_ADMIN, AuthLegend.Role.CUSTOMER_SUPPORT));

            options.AddPolicy(
                AuthLegend.Policy.CUSTOMER_ONLY,
                policyBuilder => policyBuilder.RequireClaim(ClaimTypes.PrimaryGroupSid, AuthLegend.Role.CUSTOMER));
        });

        return services;
    }
}
