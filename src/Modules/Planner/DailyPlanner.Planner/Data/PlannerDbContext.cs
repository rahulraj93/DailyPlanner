using Microsoft.EntityFrameworkCore;

namespace DailyPlanner.Planner.Data;

/// <summary>The Planner module's own DbContext. Everything it maps lives in the "planner" schema.</summary>
internal sealed class PlannerDbContext(DbContextOptions<PlannerDbContext> options) : DbContext(options)
{
    public const string Schema = "planner";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlannerDbContext).Assembly);
    }
}
