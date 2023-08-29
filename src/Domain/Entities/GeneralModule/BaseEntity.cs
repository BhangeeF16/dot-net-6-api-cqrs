using System.ComponentModel;

namespace Domain.Entities.GeneralModule;

public abstract class BaseEntity : AuditableEntity
{
    public BaseEntity() : base() => (IsActive, IsDeleted) = (true, false);

    public void DeActivateAndDelete() => (IsActive, IsDeleted) = (false, true);
    public void ActivateAndRestore() => (IsActive, IsDeleted) = (true, false);
    public void Activate() => IsActive = true;
    public void DeActivate() => IsActive = false;
    public void Delete() => IsDeleted = true;
    public void Restore() => IsDeleted = false;

    [DefaultValue(true)]
    public bool IsActive { get; set; } = true;
    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;
}
