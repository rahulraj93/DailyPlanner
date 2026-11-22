namespace DailyPlanner.SharedKernel.Domain;

/// <summary>Something that happened inside a module. Never leaves the module.</summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
