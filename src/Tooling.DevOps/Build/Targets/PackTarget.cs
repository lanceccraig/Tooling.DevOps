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

    private IEnumerable<string>? Dependencies => Options.PackProjects.Any(p => !p.ForceBuild)
        ? Bullseye.Targets.DependsOn(BuiltInBuildTargets.Build)
        : default;

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(BuiltInBuildTargets.Pack, Dependencies, forEach: Options.PackProjects, action: PackProject);

    private async Task PackProject(PackProjectDescriptor descriptor)
    {
        var noBuildOption = !descriptor.ForceBuild ? " --no-build" : string.Empty;
        await Executor
            .Run(
                "dotnet",
                $"pack {descriptor.RelativePath} -o {Options.ArtifactsDirectoryName} -c Release -v minimal --nologo{noBuildOption}")
            .ConfigureAwait(false);
    }
}
