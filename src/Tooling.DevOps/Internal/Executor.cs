using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Internal;

[ExcludeFromCodeCoverage]
internal class Executor : IExecutor
{
    public async Task Run(string name, string arguments, string? workingDirectory = default)
        => await SimpleExec.Command.RunAsync(name, args: arguments, workingDirectory: workingDirectory)
        .ConfigureAwait(false);
}
