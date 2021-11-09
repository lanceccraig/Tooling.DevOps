using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Deploy.Versioning;
using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Deploy.Targets;

internal class CreateReleaseTarget : TargetBase
{
    private readonly IVersionService _versionService;
    private readonly IReleaseService _releaseService;

    public CreateReleaseTarget(
        IExecutor executor,
        DeployOptions options,
        IVersionService versionService,
        IReleaseService releaseService)
        : base(executor, options)
    {
        _versionService = versionService;
        _releaseService = releaseService;
    }

    public override string Name => BuiltInDeployTargets.CreateRelease;

    public override void Setup(Bullseye.Targets targets)
    {
        var version = _versionService.Resolve();
        targets.Add(
            BuiltInDeployTargets.CreateRelease,
            async () => await _releaseService.Create(version)
                .ConfigureAwait(false));
    }
}
