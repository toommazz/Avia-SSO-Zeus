using Avia.SSO.Zeus.Domain.Messaging;

namespace Avia.SSO.Zeus.Infrastructure.Messaging;

public sealed class InMemoryEventBus : IEventBus
{
    public Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default) where T : IIntegrationEvent
    {
        // TODO: replace with MassTransit/RabbitMQ integration
        return Task.CompletedTask;
    }
}
