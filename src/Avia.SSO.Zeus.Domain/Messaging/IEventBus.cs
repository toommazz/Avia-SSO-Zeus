namespace Avia.SSO.Zeus.Domain.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default) where T : IIntegrationEvent;
}
