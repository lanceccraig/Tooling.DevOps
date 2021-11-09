using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class DeleteArtifactsTarget : TargetBase
{
    public DeleteArtifactsTarget(IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.DeleteArtifacts;

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(
            BuiltInBuildTargets.DeleteArtifacts,
            async () =>
            {
                await Executor
                    .Run(
                        "powershell",
                        $"-Command New-Item -Path {Options.ArtifactsDirectoryName} -ItemType Directory -Force | Out-Null")
                    .ConfigureAwait(false);

                await Executor
                    .Run("powershell", "-Command Remove-Item *", workingDirectory: Options.ArtifactsDirectoryName)
                    .ConfigureAwait(false);
            });
}
