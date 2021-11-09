namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the targets that are built-in to the deploy process.
/// </summary>
public static class BuiltInDeployTargets
{
    /// <summary>
    /// Compiles the configured InnoScript installers.
    /// </summary>
    public const string CompileInstallers = "Compile Installers";

    /// <summary>
    /// Creates a GitHub release with a summary of included issues and any configured assets.
    /// </summary>
    public const string CreateRelease = "Create Release";
}
