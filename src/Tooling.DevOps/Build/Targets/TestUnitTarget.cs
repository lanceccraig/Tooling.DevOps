using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class TestUnitTarget : TestTargetBase
{
    public const string TestSuite = BuiltInTestSuites.Unit;

    public TestUnitTarget(IExecutor executor, BuildOptions options)
        : base(BuiltInBuildTargets.TestUnit, TestSuite, executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.TestUnit;
}
