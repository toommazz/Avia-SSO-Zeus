namespace Avia.SSO.Zeus.Domain.Common;

public interface IDomainEventHandler<TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
