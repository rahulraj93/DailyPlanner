using Microsoft.EntityFrameworkCore;

namespace DailyPlanner.Life.Data;

/// <summary>The Life module's own DbContext. Everything it maps lives in the "life" schema.</summary>
internal sealed class LifeDbContext(DbContextOptions<LifeDbContext> options) : DbContext(options)
{
    public const string Schema = "life";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifeDbContext).Assembly);
    }
}
