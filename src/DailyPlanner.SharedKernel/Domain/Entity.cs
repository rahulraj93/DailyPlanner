namespace DailyPlanner.SharedKernel.Domain;

/// <summary>
/// Base class for every user-owned row.
/// Ids are Guid v7 (time-ordered). UserId and the timestamps are stamped by the
/// module's DbContext on save; DeletedAt marks a soft delete (a tombstone for sync later).
/// </summary>
public abstract class Entity
{
    public Guid Id { get; private init; } = Guid.CreateVersion7();

    public Guid UserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsDeleted => DeletedAt is not null;
}
