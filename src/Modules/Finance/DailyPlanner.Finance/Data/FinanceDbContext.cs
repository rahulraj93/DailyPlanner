using Microsoft.EntityFrameworkCore;

namespace DailyPlanner.Finance.Data;

/// <summary>The Finance module's own DbContext. Everything it maps lives in the "finance" schema.</summary>
internal sealed class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public const string Schema = "finance";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
    }
}
