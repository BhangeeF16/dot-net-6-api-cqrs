namespace Domain.Entities.GeneralModule;

public abstract class AuditableEntity
{
    public AuditableEntity() { }

    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

}
