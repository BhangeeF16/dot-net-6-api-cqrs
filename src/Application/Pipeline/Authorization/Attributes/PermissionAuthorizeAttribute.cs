using Application.Pipeline.Authorization.Extensions;
using Domain.Entities.GeneralModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Application.Pipeline.Authorization.Attributes;

public class PermitAttribute : AuthorizeAttribute
{
    public PermitAttribute(string policy) : base(policy) { }
    public PermitAttribute(PermissionLevel level, string module) : base(level.PolicyName(module)) { }

    public class PolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(RequirementPolicyExtensions.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)) return await base.GetPolicyAsync(policyName);
            return new AuthorizationPolicyBuilder().AddRequirements(policyName.ToRequirement()).Build();
        }
    }
}