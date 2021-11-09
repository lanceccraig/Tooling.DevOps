namespace LanceC.Tooling.DevOps.Internal;

/// <summary>
/// Defines the executor for commands.
/// </summary>
internal interface IExecutor
{
    /// <summary>
    /// Run a command.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="arguments">The arguments to pass to the command.</param>
    /// <param name="workingDirectory">The working directory in which to run the command.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task Run(string name, string arguments, string? workingDirectory = null);
}
