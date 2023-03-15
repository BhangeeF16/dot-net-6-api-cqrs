using Domain.Common.DomainEvent;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Private;

public class MediatrDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<MediatrDomainEventDispatcher> _log;
    public MediatrDomainEventDispatcher(IMediator mediator, ILogger<MediatrDomainEventDispatcher> log)
    {
        _mediator = mediator;
        _log = log;
    }

    public async Task Dispatch(IDomainEvent devent)
    {
        var domainEventNotification = CreateDomainEventNotification(devent);
        await _mediator.Publish(domainEventNotification);
        _log.LogDebug("Dispatching Domain Event as MediatR notification.  EventType: {eventType}", devent.GetType());
    }

    private static INotification CreateDomainEventNotification(IDomainEvent domainEvent)
    {
        var genericDispatcherType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(genericDispatcherType, domainEvent);

    }
}