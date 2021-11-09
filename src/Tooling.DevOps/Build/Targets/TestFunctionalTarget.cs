using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class TestFunctionalTarget : TestTargetBase
{
    public const string TestSuite = BuiltInTestSuites.Functional;

    public TestFunctionalTarget(IExecutor executor, BuildOptions options)
        : base(BuiltInBuildTargets.TestFunctional, TestSuite, executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.TestFunctional;
}
