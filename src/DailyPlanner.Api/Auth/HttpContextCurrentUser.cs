using System.Security.Claims;
using DailyPlanner.SharedKernel.Users;

namespace DailyPlanner.Api.Auth;

/// <summary>ICurrentUser backed by the authenticated principal of the current HTTP request.</summary>
internal sealed class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new InvalidOperationException("No signed-in user for this request.");
        }
    }
}
