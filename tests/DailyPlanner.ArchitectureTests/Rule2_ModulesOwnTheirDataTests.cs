using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace DailyPlanner.ArchitectureTests;

/// <summary>Rule 2: a module never reads or writes another module's tables or DbContext.</summary>
public sealed class Rule2_ModulesOwnTheirDataTests
{
    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void Module_has_exactly_one_DbContext_in_its_internal_project(string moduleName)
    {
        var module = Modules.Get(moduleName);

        var dbContexts = module.InternalAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DbContext))).ToList();

        Assert.Single(dbContexts);
        Assert.False(dbContexts[0].IsPublic, $"{dbContexts[0].Name} must be internal.");
    }

    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void No_other_module_uses_this_modules_DbContext(string moduleName)
    {
        var module = Modules.Get(moduleName);
        var others = Modules.All.Where(m => m != module)
            .SelectMany(m => new[] { m.InternalAssembly, m.ContractsAssembly })
            .ToArray();

        var rule = Types().That().ResideInAssembly(others[0], others[1..])
            .Should().NotDependOnAny(module.DbContextType)
            .WithoutRequiringPositiveResults();

        ArchitectureRules.AssertHolds(rule);
    }

    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void DbContext_maps_only_its_own_entities_into_its_own_schema(string moduleName)
    {
        var module = Modules.Get(moduleName);
        using var db = CreateDbContext(module);

        Assert.Equal(module.Schema, db.Model.GetDefaultSchema());

        foreach (var entityType in db.Model.GetEntityTypes())
        {
            Assert.True(
                entityType.ClrType.Assembly == module.InternalAssembly,
                $"{module.DbContextType.Name} maps {entityType.ClrType.FullName}, which belongs to another project.");

            var schema = entityType.GetSchema();
            Assert.True(
                schema == module.Schema,
                $"{entityType.ClrType.Name} is mapped to schema '{schema}', expected '{module.Schema}'.");
        }
    }

    /// <summary>Builds the module's DbContext through its real Add{Module}Module registration. No database is contacted.</summary>
    private static DbContext CreateDbContext(ModuleInfo module)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DailyPlanner"] = "Server=unused;Database=unused",
            })
            .Build();

        var services = new ServiceCollection();
        module.RegistrationType.GetMethod($"Add{module.Name}Module")!.Invoke(null, [services, configuration]);

        var scope = services.BuildServiceProvider().CreateScope();
        return (DbContext)scope.ServiceProvider.GetRequiredService(module.DbContextType);
    }
}
