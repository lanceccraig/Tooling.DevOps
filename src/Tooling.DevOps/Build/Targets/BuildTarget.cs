using LanceC.Tooling.DevOps.Internal;
using static Bullseye.Targets;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class BuildTarget : TargetBase
{
    public BuildTarget(IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.Build;

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(
            BuiltInBuildTargets.Build,
            DependsOn(BuiltInBuildTargets.Clean),
            async () => await Executor
                .Run("dotnet", "build -c Release -v minimal --nologo")
                .ConfigureAwait(false));
}
