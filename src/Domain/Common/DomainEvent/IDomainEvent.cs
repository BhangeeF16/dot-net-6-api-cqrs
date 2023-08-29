namespace Domain.Common.DomainEvent;

public interface IDomainEvent
{
    public bool IsPublished { get; }
    public DateTimeOffset DateOccurred { get; }
}
