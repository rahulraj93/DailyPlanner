namespace DailyPlanner.SharedKernel.Events;

/// <summary>Handles one integration event type. Register handlers in DI; the event bus finds them there.</summary>
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken);
}
