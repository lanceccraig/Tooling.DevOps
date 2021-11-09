using LanceC.Tooling.DevOps.Build.Targets;
using Microsoft.Extensions.DependencyInjection;

namespace LanceC.Tooling.DevOps.Build;

internal static class BuildServiceCollectionExtensions
{
    public static IServiceCollection AddBuildOptions(this IServiceCollection services, Action<BuildOptions>? optionsAction)
    {
        var options = new BuildOptions();
        optionsAction?.Invoke(options);

        return services.AddSingleton(options);
    }

    public static IServiceCollection AddBuildTargets(this IServiceCollection services)
        => services.AddTarget<DeleteArtifactsTarget>()
        .AddTarget<CleanTarget>()
        .AddTarget<BuildTarget>()
        .AddTarget<TestUnitTarget>()
        .AddTarget<TestIntegrationTarget>()
        .AddTarget<TestFunctionalTarget>()
        .AddTarget<PackTarget>()
        .AddTarget<PublishTarget>();
}
