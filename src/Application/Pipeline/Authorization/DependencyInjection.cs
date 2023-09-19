using Application.Pipeline.Authentication.Extensions;
using Application.Pipeline.Authorization.Attributes;
using Application.Pipeline.Authorization.Extensions;
using Application.Pipeline.Authorization.Requirements;
using Domain.Abstractions.IRepositories.IGeneric;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Application.Pipeline.Authorization;

public static class DependencyInjection
{
    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var permissions = unitOfWork.Roles.GetRolePermissions();

            services.AddAuthorization(options =>
            {
                foreach (var permission in permissions) options.AddPolicy(permission.PolicyName(), policyBuilder => policyBuilder.Requirements.Add(permission.ToRequirement()));
            });
        }

        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermitAttribute.PolicyProvider>();

        return services;
    }

    public static IServiceCollection AddRolePolicies(this IServiceCollection services)
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
