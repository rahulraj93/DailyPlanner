using DailyPlanner.SharedKernel.Users;
using Microsoft.AspNetCore.Authentication;

namespace DailyPlanner.Api.Auth;

internal static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddDailyPlannerAuth(this IServiceCollection services, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            // Fail fast rather than ever running the fixed test user outside Development.
            throw new InvalidOperationException(
                "Only the development auth handler exists so far. Configure a real provider before running outside Development.");
        }

        services
            .AddAuthentication(DevelopmentAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthHandler>(DevelopmentAuthHandler.SchemeName, null);

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

        return services;
    }
}
