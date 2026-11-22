namespace DailyPlanner.SharedKernel.Time;

/// <summary>The current instant. Inject this instead of calling DateTimeOffset.UtcNow so tests can control time.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
