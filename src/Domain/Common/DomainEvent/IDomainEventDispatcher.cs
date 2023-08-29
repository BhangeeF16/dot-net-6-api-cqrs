namespace Domain.Common.DomainEvent;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent devent);
}
