using Ardalis.GuardClauses;
using LanceC.Tooling.DevOps.Build;
using LanceC.Tooling.DevOps.Deploy;
using LanceC.Tooling.DevOps.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides extensions for configuring the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a pipeline target.
    /// </summary>
    /// <typeparam name="TTarget">The type of target to add.</typeparam>
    /// <param name="services">The service collection to modify.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddTarget<TTarget>(this IServiceCollection services)
        where TTarget : class, ITarget
        => services.AddSingleton<ITarget, TTarget>();

    /// <summary>
    /// Adds the infrastructure for running a build process.
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <param name="optionsAction">The optional action used to specify build options.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddBuildTooling(
        this IServiceCollection services,
        Action<BuildOptions>? optionsAction = default)
    {
        Guard.Against.Null(services, nameof(services));

        return services.AddSingleton<IExecutor, Executor>()
            .AddBuildOptions(optionsAction)
            .AddBuildTargets();
    }

    /// <summary>
    /// Adds the infrastructure for running a deploy process.
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <param name="repositoryInfo">The information about the repository.</param>
    /// <param name="optionsAction">The optional action used to specify deploy options.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddDeployTooling(
        this IServiceCollection services,
        RepositoryInfo repositoryInfo,
        Action<DeployOptions>? optionsAction = default)
    {
        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(repositoryInfo, nameof(repositoryInfo));

        return services.AddSingleton<IExecutor, Executor>()
            .AddSingleton<IEnvironmentAccessor, EnvironmentAccessor>()
            .AddSingleton(repositoryInfo)
            .AddDeployOptions(optionsAction)
            .AddDeployTargets()
            .AddDeployServices();
    }
}
