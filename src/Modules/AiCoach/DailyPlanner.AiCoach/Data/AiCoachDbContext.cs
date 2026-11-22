using Microsoft.EntityFrameworkCore;

namespace DailyPlanner.AiCoach.Data;

/// <summary>The AiCoach module's own DbContext. Everything it maps lives in the "ai" schema.</summary>
internal sealed class AiCoachDbContext(DbContextOptions<AiCoachDbContext> options) : DbContext(options)
{
    public const string Schema = "ai";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AiCoachDbContext).Assembly);
    }
}
