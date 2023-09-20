using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Entities.GeneralModule;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Application.Pipeline.Authorization.Requirements;

public record PermissionRequirement(string Model, PermissionLevel PermissionLevel) : IAuthorizationRequirement;
public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUnitOfWork _unitOfWork;
    public PermissionRequirementHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (await IsAllowedAsync(context.User, requirement)) context.Succeed(requirement);

        context.Fail();
        await Task.CompletedTask;
        return;
    }

    private async Task<bool> IsAllowedAsync(ClaimsPrincipal user, PermissionRequirement requirement)
    {
        if (user.HasClaim(claim => claim.Type == ClaimTypes.Role))
        {
            var roleID = Convert.ToInt32(user.Claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty);
            var role = await _unitOfWork.Roles.GetFirstOrDefaultNoTrackingAsync(x => x.ID == roleID, x => x.Permissions, x => x.Permissions.Select(y => y.Module));

            if (role.Permissions != null && role.Permissions.Any()) return role.Permissions.Any(x => x.PermissionLevel == requirement.PermissionLevel && x.Module.Name == requirement.Model);
        }
        return false;
    }
}
