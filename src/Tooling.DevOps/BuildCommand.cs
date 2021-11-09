using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using LanceC.CommandLine;
using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the application command for building the repository.
/// </summary>
[ExcludeFromCodeCoverage]
public class BuildCommand : Command
{
    /// <summary>
    /// Gets the build targets to run.
    /// </summary>
    public static readonly Option<IReadOnlyCollection<string>> TargetsOption =
        new(new[] { "--targets", "-t", }, "The targets to run.");

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildCommand"/> class.
    /// </summary>
    public BuildCommand()
        : base("build", "Builds the application(s).")
    {
        AddOption(TargetsOption);
    }

    internal class CommandHandler : ICommandHandler
    {
        private readonly IEnumerable<ITarget> _targets;

        public CommandHandler(IEnumerable<ITarget> targets)
        {
            _targets = targets;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var targetNames = context.ParseResult.ValueForOption(TargetsOption);

            var runnableTargets = _targets.SetupAll();
            await runnableTargets.RunWithoutExitingAsync(targetNames, messageOnly: e => e is SimpleExec.ExitCodeException)
                .ConfigureAwait(false);

            return CommandCode.Success;
        }
    }
}
