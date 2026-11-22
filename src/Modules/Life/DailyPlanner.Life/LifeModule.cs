using DailyPlanner.Life.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DailyPlanner.Life;

/// <summary>The only public type in this project: how DailyPlanner.Api plugs the Life module in.</summary>
public static class LifeModule
{
    public static IServiceCollection AddLifeModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DailyPlanner");

        services.AddDbContext<LifeDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__EFMigrationsHistory", LifeDbContext.Schema)));

        return services;
    }

    public static IEndpointRouteBuilder MapLifeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/life")
            .WithTags("Life")
            .RequireAuthorization();

        // Endpoints go here as the module grows.
        _ = group;

        return app;
    }
}
