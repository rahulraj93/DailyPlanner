using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace DailyPlanner.ArchitectureTests;

/// <summary>
/// Rule 6: internal types are internal; only Contracts types are public.
/// The one exception is the module's registration class (e.g. PlannerModule), which DailyPlanner.Api calls.
/// </summary>
public sealed class Rule6_OnlyContractsArePublicTests
{
    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void Internal_project_exposes_only_its_registration_class(string moduleName)
    {
        var module = Modules.Get(moduleName);

        var rule = Types().That().ResideInAssembly(module.InternalAssembly)
            .And().AreNot(module.RegistrationType)
            .Should().NotBePublic()
            .WithoutRequiringPositiveResults();

        ArchitectureRules.AssertHolds(rule);
    }

    [Theory]
    [MemberData(nameof(Modules.Names), MemberType = typeof(Modules))]
    public void Registration_class_is_public_static_and_named_after_the_module(string moduleName)
    {
        var type = Modules.Get(moduleName).RegistrationType;

        Assert.True(type.IsPublic && type.IsAbstract && type.IsSealed, $"{type.Name} must be a public static class.");
        Assert.Equal($"{moduleName}Module", type.Name);
        Assert.NotNull(type.GetMethod($"Add{moduleName}Module"));
        Assert.NotNull(type.GetMethod($"Map{moduleName}Endpoints"));
    }
}
