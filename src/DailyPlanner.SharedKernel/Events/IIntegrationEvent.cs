namespace DailyPlanner.SharedKernel.Events;

/// <summary>An event one module publishes for others. Declared in the publishing module's Contracts project.</summary>
public interface IIntegrationEvent
{
    Guid Id { get; }

    DateTimeOffset OccurredAt { get; }
}
