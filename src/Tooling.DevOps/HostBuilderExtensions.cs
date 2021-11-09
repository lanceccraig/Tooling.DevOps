using System.CommandLine.Hosting;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;

namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides extensions for configuring the host builder.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HostBuilderExtensions
{
    /// <summary>
    /// Uses the build command in the application.
    /// </summary>
    /// <param name="host">The builder for the host which will use the command.</param>
    /// <returns>The modified host builder.</returns>
    public static IHostBuilder UseBuildCommand(this IHostBuilder host)
        => host.UseCommandHandler<BuildCommand, BuildCommand.CommandHandler>();

    /// <summary>
    /// Uses the deploy command in the application.
    /// </summary>
    /// <param name="host">The builder for the host which will use the command.</param>
    /// <returns>The modified host builder.</returns>
    public static IHostBuilder UseDeployCommand(this IHostBuilder host)
        => host.UseCommandHandler<DeployCommand, DeployCommand.CommandHandler>();
}
