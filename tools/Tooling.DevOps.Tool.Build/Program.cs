using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using LanceC.CommandLine.Help;
using LanceC.Tooling.DevOps;

namespace LanceC.Tooling.DevOps.Tool.Build;

public static class Program
{
    public static async Task<int> Main(string[] args)
        => await new CommandLineBuilder(new BuildCommand())
        .UseDefaults()
        .UseHelpBuilder(context => new CommandTreeHelpBuilder(context.Console))
        .UseHost(host => host
            .ConfigureServices(
                (hostingContext, services) => services
                    .AddBuildTooling(
                        options =>
                        {
                            options.AddTestProject("tests/Tooling.DevOps.Facts", BuiltInTestSuites.Unit);
                            options.AddTestProject("tests/Tooling.DevOps.IntegrationTests", BuiltInTestSuites.Integration);
                            options.AddPackProject("src/Tooling.DevOps");
                        }))
            .UseBuildCommand())
        .Build()
        .InvokeAsync(args)
        .ConfigureAwait(false);
}
