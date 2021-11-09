using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class TestIntegrationTarget : TestTargetBase
{
    public const string TestSuite = BuiltInTestSuites.Integration;

    public TestIntegrationTarget(IExecutor executor, BuildOptions options)
        : base(BuiltInBuildTargets.TestIntegration, TestSuite, executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.TestIntegration;
}
