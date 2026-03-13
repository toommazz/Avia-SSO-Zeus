namespace Avia.SSO.Zeus.Domain.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredAt { get; }
}
