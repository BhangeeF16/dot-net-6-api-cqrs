using Application.Pipeline.Authorization.Extensions;
using Domain.Entities.GeneralModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Application.Pipeline.Authorization.Attributes;

public class PermitAttribute : AuthorizeAttribute
{
    public PermitAttribute(PermissionLevel Level, string Model) => Policy = Level.PolicyName(Model);

    public class PolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(RequirementPolicyExtensions.PolicyPrefix, StringComparison.OrdinalIgnoreCase)) return await base.GetPolicyAsync(policyName);
            return new AuthorizationPolicyBuilder().AddRequirements(policyName.ToRequirement()).Build();
        }
    }
}