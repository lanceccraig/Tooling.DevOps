using System.IO.Abstractions;
using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Deploy.Targets;
using LanceC.Tooling.DevOps.Deploy.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace LanceC.Tooling.DevOps.Deploy;

internal static class DeployServiceCollectionExtensions
{
    public static IServiceCollection AddDeployOptions(this IServiceCollection services, Action<DeployOptions>? optionsAction)
    {
        var options = new DeployOptions();
        optionsAction?.Invoke(options);

        return services.AddSingleton(options);
    }

    public static IServiceCollection AddDeployTargets(this IServiceCollection services)
        => services.AddTarget<CompileInstallerTarget>()
        .AddTarget<CreateReleaseTarget>();

    public static IServiceCollection AddDeployServices(this IServiceCollection services)
        => services.AddSingleton<IFileSystem, FileSystem>()
        .AddSingleton<ICredentialStore, LocalCredentialStore>()
        .AddSingleton<IGitHubClientFactory, GitHubClientFactory>()
        .AddSingleton<IReleasableIssueConversionService, ReleasableIssueConversionService>()
        .AddSingleton<IReleaseService, ReleaseService>()
        .AddSingleton<IVersionResolutionStrategy, AssemblyVersionResolutionStrategy>()
        .AddSingleton<IVersionResolutionStrategy, MSBuildVersionResolutionStrategy>()
        .AddSingleton<IRepositoryService, RepositoryService>()
        .AddSingleton<IVersionService, VersionService>();
}
