using Domain.IContracts.IRepositories.IGenericRepositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Application.Pipeline.Authorization.PolicyAuth.PermissionsRequirementPolicy
{
    public class IsAllowedRequirementHandler : IAuthorizationHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        public IsAllowedRequirementHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                if (requirement is IsAllowedRequirement)
                {
                    if (IsAllowed(context.User, (IsAllowedRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
        private bool IsAllowed(ClaimsPrincipal user, IsAllowedRequirement requirement)
        {
            if (user.HasClaim(claim => claim.Type == ClaimTypes.Role))
            {
                var RoleId = Convert.ToInt32(user.Claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty);
                var thisRole = _unitOfWork.Roles.GetFirstOrDefaultAsync(x => x.ID == RoleId).Result;

                var thisRolesThisPermission = thisRole.GetType().GetProperty(requirement.PermissionName);

                var IsAllowed = thisRolesThisPermission?.GetValue(thisRole, null);

                if (Convert.ToBoolean(IsAllowed ?? false))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
