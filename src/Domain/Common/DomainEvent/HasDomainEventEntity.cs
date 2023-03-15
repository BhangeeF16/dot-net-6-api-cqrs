using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common.DomainEvent;

public abstract class HasDomainEventEntity : IHasDomainEventEntity
{
    [NotMapped]
    private readonly ConcurrentQueue<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IProducerConsumerCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void PublishEvent(IDomainEvent @event)
    {
        _domainEvents.Enqueue(@event);
    }

    protected static Guid NewIdGuid() => Guid.NewGuid();
}