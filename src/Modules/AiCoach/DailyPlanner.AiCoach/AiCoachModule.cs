using DailyPlanner.AiCoach.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DailyPlanner.AiCoach;

/// <summary>The only public type in this project: how DailyPlanner.Api plugs the AiCoach module in.</summary>
public static class AiCoachModule
{
    public static IServiceCollection AddAiCoachModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DailyPlanner");

        services.AddDbContext<AiCoachDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory", AiCoachDbContext.Schema)));

        return services;
    }

    public static IEndpointRouteBuilder MapAiCoachEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ai-coach")
            .WithTags("AiCoach")
            .RequireAuthorization();

        // Endpoints go here as the module grows.
        _ = group;

        return app;
    }
}
