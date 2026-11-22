namespace DailyPlanner.ArchitectureTests;

/// <summary>Finds source files (csproj) by walking up from the test output folder to DailyPlanner.sln.</summary>
internal static class RepositoryFiles
{
    private static readonly DirectoryInfo Root = FindRoot();

    public static FileInfo FindProject(string projectName) =>
        Root.GetDirectories("src").Single()
            .EnumerateFiles($"{projectName}.csproj", SearchOption.AllDirectories)
            .Single();

    private static DirectoryInfo FindRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "DailyPlanner.sln")))
            {
                return dir;
            }
        }

        throw new InvalidOperationException("Could not find DailyPlanner.sln above " + AppContext.BaseDirectory);
    }
}
