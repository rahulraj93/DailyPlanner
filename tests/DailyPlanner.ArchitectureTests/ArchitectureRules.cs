using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;

namespace DailyPlanner.ArchitectureTests;

/// <summary>Loads every module assembly once (ArchUnitNET reads their IL) and checks rules with readable failures.</summary>
internal static class ArchitectureRules
{
    public static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
        [
            Modules.SharedKernel,
            .. Modules.All.Select(m => m.InternalAssembly),
            .. Modules.All.Select(m => m.ContractsAssembly),
        ])
        .Build();

    public static void AssertHolds(IArchRule rule)
    {
        var failures = rule.Evaluate(Architecture)
            .Where(result => !result.Passed)
            .Select(result => result.Description)
            .ToList();

        Assert.True(failures.Count == 0, $"Rule broken: {rule.Description}{Environment.NewLine}{string.Join(Environment.NewLine, failures)}");
    }
}
