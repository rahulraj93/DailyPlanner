using System.Xml.Linq;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace DailyPlanner.ArchitectureTests;

/// <summary>Rule 1: a module may reference another module's *.Contracts project only, never its internal project.</summary>
public sealed class Rule1_ModulesReferenceOnlyContractsTests
{
    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void Internal_project_references_only_SharedKernel_and_Contracts_projects(string moduleName)
    {
        var references = ProjectReferencesOf($"DailyPlanner.{moduleName}");

        var forbidden = references
            .Where(r => r != "DailyPlanner.SharedKernel" && !r.EndsWith(".Contracts", StringComparison.Ordinal))
            .ToList();

        Assert.True(forbidden.Count == 0, $"DailyPlanner.{moduleName} must not reference: {string.Join(", ", forbidden)}");
        Assert.Contains($"DailyPlanner.{moduleName}.Contracts", references);
    }

    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void Contracts_project_references_only_SharedKernel_and_other_Contracts_projects(string moduleName)
    {
        var references = ProjectReferencesOf($"DailyPlanner.{moduleName}.Contracts");

        var forbidden = references
            .Where(r => r != "DailyPlanner.SharedKernel" && !r.EndsWith(".Contracts", StringComparison.Ordinal))
            .ToList();

        Assert.True(forbidden.Count == 0, $"DailyPlanner.{moduleName}.Contracts must not reference: {string.Join(", ", forbidden)}");
    }

    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void Module_code_does_not_use_types_from_other_modules_internal_projects(string moduleName)
    {
        // The csproj checks above catch the reference; this catches actual use in compiled code (IL).
        var module = Modules.Get(moduleName);
        var otherInternals = Modules.All.Where(m => m != module).Select(m => m.InternalAssembly).ToArray();

        var rule = Types().That().ResideInAssembly(module.InternalAssembly, module.ContractsAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(otherInternals[0], otherInternals[1..]))
            .WithoutRequiringPositiveResults();

        ArchitectureRules.AssertHolds(rule);
    }

    private static List<string> ProjectReferencesOf(string projectName)
    {
        var project = RepositoryFiles.FindProject(projectName);

        return XDocument.Load(project.FullName)
            .Descendants("ProjectReference")
            .Select(e => Path.GetFileNameWithoutExtension(e.Attribute("Include")!.Value.Replace('\\', '/')))
            .ToList();
    }
}
