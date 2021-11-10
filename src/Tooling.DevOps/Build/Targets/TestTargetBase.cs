using LanceC.Tooling.DevOps.Internal;
using static Bullseye.Targets;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal abstract class TestTargetBase : TargetBase
{
    private readonly string _targetName;
    private readonly string _testSuite;

    protected TestTargetBase(string targetName, string testSuite, IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
        _targetName = targetName;
        _testSuite = testSuite;
    }

    public override bool PreCondition => TestProjects.Any();

    private IEnumerable<TestProjectDescriptor> TestProjects
        => Options.TestProjects.Where(p => p.TestSuite.Equals(_testSuite, StringComparison.OrdinalIgnoreCase));

    private IEnumerable<string>? Dependencies => TestProjects.Any(p => !p.ForceBuild)
        ? DependsOn(BuiltInBuildTargets.Build)
        : default;

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(_targetName, Dependencies, forEach: TestProjects, action: TestProject);

    private async Task TestProject(TestProjectDescriptor descriptor)
    {
        var noBuildOption = !descriptor.ForceBuild ? " --no-build" : string.Empty;
        await Executor
            .Run("dotnet", $"test {descriptor.RelativePath} -c Release -v minimal --nologo{noBuildOption}")
            .ConfigureAwait(false);
    }
}
