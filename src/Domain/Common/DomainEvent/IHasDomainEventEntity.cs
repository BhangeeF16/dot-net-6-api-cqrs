using System.Collections.Concurrent;

namespace Domain.Common.DomainEvent;

public interface IHasDomainEventEntity
{
    IProducerConsumerCollection<IDomainEvent> DomainEvents { get; }
}
