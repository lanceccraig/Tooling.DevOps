using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal class PublishTarget : TargetBase
{
    public PublishTarget(IExecutor executor, BuildOptions options)
        : base(executor, options)
    {
    }

    public override string Name => BuiltInBuildTargets.Publish;

    public override bool PreCondition => Options.PublishProjects.Any();

    private IEnumerable<string>? Dependencies => Options.PublishProjects.Any(p => !p.ForceBuild)
        ? Bullseye.Targets.DependsOn(BuiltInBuildTargets.Build)
        : default;

    public override void Setup(Bullseye.Targets targets)
        => targets.Add(BuiltInBuildTargets.Publish, Dependencies, forEach: Options.PublishProjects, action: PublishProject);

    private async Task PublishProject(PublishProjectDescriptor descriptor)
    {
        var noBuildOption = !descriptor.ForceBuild ? " --no-build" : string.Empty;
        await Executor
            .Run(
                "dotnet",
                $"publish {descriptor.RelativePath} -p:PublishProfile={descriptor.ProfileName} -c Release -v minimal --nologo{noBuildOption}")
            .ConfigureAwait(false);
    }
}
