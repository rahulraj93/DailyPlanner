using DailyPlanner.Notify.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DailyPlanner.Notify;

/// <summary>The only public type in this project: how DailyPlanner.Api plugs the Notify module in.</summary>
public static class NotifyModule
{
    public static IServiceCollection AddNotifyModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DailyPlanner");

        services.AddDbContext<NotifyDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory", NotifyDbContext.Schema)));

        return services;
    }

    public static IEndpointRouteBuilder MapNotifyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notify")
            .WithTags("Notify")
            .RequireAuthorization();

        // Endpoints go here as the module grows.
        _ = group;

        return app;
    }
}
