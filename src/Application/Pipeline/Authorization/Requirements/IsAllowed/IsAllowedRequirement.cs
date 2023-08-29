using Microsoft.AspNetCore.Authorization;

namespace Application.Pipeline.Authorization.Requirements.IsAllowed
{
    public class IsAllowedRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; set; }

        public IsAllowedRequirement(string permissionName)
        {
            PermissionName = permissionName;
        }
    }
}
