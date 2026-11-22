namespace DailyPlanner.SharedKernel.Users;

/// <summary>
/// The signed-in user for the current request. Implemented in DailyPlanner.Api,
/// so modules never know which auth provider is in use.
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    /// <summary>The user's id. Throws if nobody is signed in.</summary>
    Guid UserId { get; }
}
