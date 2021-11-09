namespace LanceC.Tooling.DevOps.Deploy.Versioning;

/// <summary>
/// Defines the strategy for resolving the repository version.
/// </summary>
internal interface IVersionResolutionStrategy
{
    /// <summary>
    /// Gets the kind of strategy.
    /// </summary>
    VersionResolutionStrategy Kind { get; }

    /// <summary>
    /// Resolves the repository version.
    /// </summary>
    /// <returns>The version string.</returns>
    string Resolve();
}
