using DailyPlanner.Planner.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DailyPlanner.Planner;

/// <summary>The only public type in this project: how DailyPlanner.Api plugs the Planner module in.</summary>
public static class PlannerModule
{
    public static IServiceCollection AddPlannerModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DailyPlanner");

        services.AddDbContext<PlannerDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory", PlannerDbContext.Schema)));

        return services;
    }

    public static IEndpointRouteBuilder MapPlannerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/planner")
            .WithTags("Planner")
            .RequireAuthorization();

        // Endpoints go here as the module grows.
        _ = group;

        return app;
    }
}
