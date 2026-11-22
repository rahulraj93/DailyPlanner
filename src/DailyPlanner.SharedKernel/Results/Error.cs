namespace DailyPlanner.SharedKernel.Results;

/// <summary>A failure with a stable code (for clients/tests) and a human-readable message.</summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string what) => new("not_found", $"{what} was not found.");

    public static Error Validation(string message) => new("validation", message);
}
