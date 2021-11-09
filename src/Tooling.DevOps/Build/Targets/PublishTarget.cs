using LanceC.Tooling.DevOps.Internal;
using static Bullseye.Targets;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class PublishTarget : TargetBase
{
    public PublishTarget(IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.Publish;

    public override bool PreCondition => Options.PublishProjects.Any();

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(
            BuiltInBuildTargets.Publish,
            DependsOn(BuiltInBuildTargets.Build),
            forEach: Options.PublishProjects,
            action: async descriptor => await Executor
                .Run(
                    "dotnet",
                    $"publish {descriptor.RelativePath} -p:PublishProfile={descriptor.ProfileName} -c Release -v minimal --no-build --nologo")
                .ConfigureAwait(false));
}
