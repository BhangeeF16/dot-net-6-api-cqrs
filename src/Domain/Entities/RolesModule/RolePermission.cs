using Domain.Entities.GeneralModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities.RolesModule;

[Table("RolePermission")]
public class RolePermission : BaseEntity, IEquatable<RolePermission>
{
    public RolePermission() : base() { }

    [Key]
    public int ID { get; set; }

    public PermissionLevel PermissionLevel { get; set; }

    [ForeignKey("Module")]
    public int fk_ModuleID { get; set; }
    public virtual Module? Module { get; set; }

    [ForeignKey("Role")]
    public int fk_RoleID { get; set; }
    public virtual Role? Role { get; set; }

    public bool Equals(RolePermission? other)
    {
        if (this == null || other == null) return false;

        return fk_ModuleID == other.fk_ModuleID &&
               fk_RoleID == other.fk_RoleID &&
               PermissionLevel == other.PermissionLevel;
    }
    public class EqualityComparer : IEqualityComparer<RolePermission>
    {
        #region IEqualityComparer<T> Members

        public bool Equals(RolePermission? x, RolePermission? y)
        {
            if (x == null || y == null) return false;

            return x.fk_ModuleID == y.fk_ModuleID &&
                   x.fk_RoleID == y.fk_RoleID &&
                   x.PermissionLevel == y.PermissionLevel;
        }

        public int GetHashCode([DisallowNull] RolePermission obj)
        {
            return obj.ID;
        }

        #endregion
    }
}