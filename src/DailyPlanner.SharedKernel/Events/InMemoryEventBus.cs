namespace DailyPlanner.SharedKernel.Events;

/// <summary>
/// Simple in-process bus: resolves every IIntegrationEventHandler&lt;TEvent&gt; from DI
/// and awaits them one after another. Register it as scoped so handlers share the request scope.
/// Uses only System.IServiceProvider, so SharedKernel needs no package references.
/// </summary>
public sealed class InMemoryEventBus(IServiceProvider services) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var handlers = services.GetService(typeof(IEnumerable<IIntegrationEventHandler<TEvent>>))
            as IEnumerable<IIntegrationEventHandler<TEvent>>;

        foreach (var handler in handlers ?? [])
        {
            await handler.HandleAsync(integrationEvent, cancellationToken);
        }
    }
}
