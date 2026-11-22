using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DailyPlanner.Api.Auth;

/// <summary>
/// Development only: signs every request in as one fixed test user.
/// Replaced by a real external provider (JWT) later; modules only ever see ICurrentUser.
/// </summary>
internal sealed class DevelopmentAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Development";

    public static readonly Guid TestUserId = Guid.Parse("01900000-0000-7000-8000-000000000001");

    public const string TestUserName = "Test User";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, TestUserId.ToString()),
            new(ClaimTypes.Name, TestUserName),
        ];

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}
