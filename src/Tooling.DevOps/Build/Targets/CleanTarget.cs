using LanceC.Tooling.DevOps.Internal;
using static Bullseye.Targets;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class CleanTarget : TargetBase
{
    public CleanTarget(IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.Clean;

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(
            BuiltInBuildTargets.Clean,
            async () => await Executor
                .Run("dotnet", "clean -c Release -v minimal --nologo")
                .ConfigureAwait(false));
}
