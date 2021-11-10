using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using LanceC.CommandLine.Help;
using LanceC.Tooling.DevOps;

namespace LanceC.Tooling.DevOps.Tool.Deploy;

public static class Program
{
    public static async Task<int> Main(string[] args)
        => await new CommandLineBuilder(new DeployCommand())
        .UseDefaults()
        .UseHelpBuilder(context => new CommandTreeHelpBuilder(context.Console))
        .UseHost(host => host
            .ConfigureServices(
                (hostingContext, services) => services
                    .AddDeployTooling(
                        new RepositoryInfo("Tooling.DevOps", "lanceccraig"),
                        options =>
                        {
                            options.ReleaseAssetRelativePaths.Add("LanceC.Tooling.DevOps.{version}.nupkg");
                            options.ReleaseAssetRelativePaths.Add("LanceC.Tooling.DevOps.{version}.snupkg");
                        }))
            .UseDeployCommand())
        .Build()
        .InvokeAsync(args)
        .ConfigureAwait(false);
}
