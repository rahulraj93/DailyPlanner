using Microsoft.EntityFrameworkCore;

namespace DailyPlanner.Notify.Data;

/// <summary>The Notify module's own DbContext. Everything it maps lives in the "notify" schema.</summary>
internal sealed class NotifyDbContext(DbContextOptions<NotifyDbContext> options) : DbContext(options)
{
    public const string Schema = "notify";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotifyDbContext).Assembly);
    }
}
