using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class PackTarget : TargetBase
{
    public PackTarget(IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.Pack;

    public override bool PreCondition => Options.PackProjects.Any();

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(
            BuiltInBuildTargets.Pack,
            forEach: Options.PackProjects,
            action: async descriptor => await Executor
                .Run(
                    "dotnet",
                    $"pack {descriptor.RelativePath} -o {Options.ArtifactsDirectoryName} -c Release -v minimal --nologo")
                .ConfigureAwait(false));
}
