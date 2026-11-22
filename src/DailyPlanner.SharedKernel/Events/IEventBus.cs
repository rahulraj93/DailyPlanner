namespace DailyPlanner.SharedKernel.Events;

/// <summary>Publishes integration events to the handlers of other modules, in process.</summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
