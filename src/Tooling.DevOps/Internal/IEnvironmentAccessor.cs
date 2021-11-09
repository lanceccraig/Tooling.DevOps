namespace LanceC.Tooling.DevOps.Internal;

/// <summary>
/// Defines the accessor for the current environment.
/// </summary>
internal interface IEnvironmentAccessor
{
    /// <summary>
    /// Gets the path for a special folder.
    /// </summary>
    /// <param name="folder">The folder to get the path for.</param>
    /// <returns>The special folder path.</returns>
    string GetFolderPath(Environment.SpecialFolder folder);

    /// <summary>
    /// Gets an environment variable.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="target">The target variable scope.</param>
    /// <returns>The environment variable if it is found; otherwise, <c>null</c>.</returns>
    string? GetVariable(string name, EnvironmentVariableTarget target);
}
