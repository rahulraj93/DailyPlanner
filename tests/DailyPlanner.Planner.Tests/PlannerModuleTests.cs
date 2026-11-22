using DailyPlanner.Planner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DailyPlanner.Planner.Tests;

public sealed class PlannerModuleTests
{
    [Fact]
    public void AddPlannerModule_registers_a_DbContext_in_the_planner_schema()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DailyPlanner"] = "Server=unused;Database=unused",
            })
            .Build();

        using var provider = new ServiceCollection()
            .AddPlannerModule(configuration)
            .BuildServiceProvider();

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlannerDbContext>();
        var relational = RelationalOptionsExtension.Extract(db.GetService<IDbContextOptions>());

        Assert.Equal("planner", db.Model.GetDefaultSchema());
        Assert.Equal("planner", relational.MigrationsHistoryTableSchema);
    }
}
