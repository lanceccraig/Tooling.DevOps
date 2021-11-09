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

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(
            _targetName,
            DependsOn(BuiltInBuildTargets.Build),
            forEach: TestProjects,
            action: async descriptor => await Executor
                .Run("dotnet", $"test {descriptor.RelativePath} -c Release -v minimal --no-build --nologo")
                .ConfigureAwait(false));
}
