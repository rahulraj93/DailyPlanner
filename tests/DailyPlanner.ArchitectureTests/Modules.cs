using System.Reflection;
using DailyPlanner.AiCoach;
using DailyPlanner.Finance;
using DailyPlanner.Life;
using DailyPlanner.Notify;
using DailyPlanner.Planner;
using DailyPlanner.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace DailyPlanner.ArchitectureTests;

/// <summary>One module: its internal project, its Contracts project, its registration class and its schema.</summary>
public sealed record ModuleInfo(string Name, string Schema, Type RegistrationType)
{
    public Assembly InternalAssembly => RegistrationType.Assembly;

    public Assembly ContractsAssembly => Assembly.Load($"DailyPlanner.{Name}.Contracts");

    public Type DbContextType => InternalAssembly.GetTypes().Single(t => t.IsSubclassOf(typeof(DbContext)));

    public override string ToString() => Name;
}

/// <summary>The catalogue of modules. Add a line here when a module is added.</summary>
public static class Modules
{
    public static readonly IReadOnlyList<ModuleInfo> All =
    [
        new("Planner", "planner", typeof(PlannerModule)),
        new("Life", "life", typeof(LifeModule)),
        new("Finance", "finance", typeof(FinanceModule)),
        new("Notify", "notify", typeof(NotifyModule)),
        new("AiCoach", "ai", typeof(AiCoachModule)),
    ];

    public static Assembly SharedKernel => typeof(Result).Assembly;

    public static ModuleInfo Get(string name) => All.Single(m => m.Name == name);

    /// <summary>Module names as xUnit theory data, so each module shows up as its own test case.</summary>
    public static TheoryData<string> Names => [.. All.Select(m => m.Name)];
}
