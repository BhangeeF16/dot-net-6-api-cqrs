using Application.Pipeline.Authorization.Requirements;
using Domain.Common.Extensions;
using Domain.Entities.GeneralModule;
using Domain.Entities.RolesModule;
using System.Text;

namespace Application.Pipeline.Authorization.Extensions;
public static class RequirementPolicyExtensions
{
    internal const string SEPERATOR = "_";
    internal const string POLICY_PREFIX = "CAN_";

    public static PermissionRequirement ToRequirement(this PermissionLevel permissionLevel, string model) => new(model, permissionLevel);
    public static PermissionRequirement ToRequirement(this RolePermission rolePermission) => new(rolePermission.Module.Name, rolePermission.PermissionLevel);
    public static PermissionRequirement ToRequirement(this string policyName) => new(policyName.Model(), policyName.PermissionLevel());

    public static string Model(this string policyName)
    {
        string[] parts = policyName.Split('_');

        if (parts.Length < 3) throw new ArgumentException("Invalid Policy Name format");

        return parts.Last();
    }
    public static PermissionLevel PermissionLevel(this string policyName)
    {
        string[] parts = policyName.Split('_');
        
        if (parts.Length < 3) throw new ArgumentException("Invalid Policy Name format");

        string permissionDescription = string.Join("_", parts.Skip(1).Take(parts.Length - 2));
        foreach (PermissionLevel level in Enum.GetValues(typeof(PermissionLevel))) if (level.GetDescription() == permissionDescription) return level;

        throw new ArgumentException("Invalid permission description");
    }

    public static string PolicyName(this PermissionLevel permissionLevel, string model) 
        => new StringBuilder().AppendFormat("{0}{2}{1}{3}", POLICY_PREFIX, SEPERATOR, permissionLevel.GetDescription(), model).ToString().ToUpperInvariant();
    public static string PolicyName(this RolePermission rolePermission) 
        => new StringBuilder().AppendFormat("{0}{2}{1}{3}", POLICY_PREFIX, SEPERATOR, rolePermission.PermissionLevel.GetDescription(), rolePermission.Module.Name).ToString().ToUpperInvariant();
}
