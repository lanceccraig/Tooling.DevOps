using System.IO.Abstractions;
using LanceC.Tooling.DevOps.Deploy.Versioning;
using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Deploy.Targets;

internal class CompileInstallerTarget : TargetBase
{
    private readonly IVersionService _versionService;
    private readonly IFileSystem _fileSystem;

    public CompileInstallerTarget(IExecutor executor, DeployOptions options, IVersionService versionService, IFileSystem fileSystem)
        : base(executor, options)
    {
        _versionService = versionService;
        _fileSystem = fileSystem;
    }

    public override string Name => BuiltInDeployTargets.CompileInstallers;

    public override bool PreCondition => Options.InstallerScriptRelativePaths.Any();

    public override void Setup(Bullseye.Targets targets)
    {
        var buildDirectoryPath = _fileSystem.Path.Combine(_fileSystem.Directory.GetCurrentDirectory(), "build");
        var version = _versionService.Resolve();
        targets.Add(
            BuiltInDeployTargets.CompileInstallers,
            forEach: Options.InstallerScriptRelativePaths,
            async installerScriptRelativePath =>
            {
                var installerScriptFilePath = _fileSystem.Path.Combine(buildDirectoryPath, installerScriptRelativePath);
                await Executor.Run("iscc", $"\"{installerScriptFilePath}\" /DVERSION={version}")
                    .ConfigureAwait(false);
            });
    }
}
