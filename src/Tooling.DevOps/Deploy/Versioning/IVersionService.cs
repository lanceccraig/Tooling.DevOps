namespace LanceC.Tooling.DevOps.Deploy.Versioning;

/// <summary>
/// Defines the service for determining the repository version.
/// </summary>
internal interface IVersionService
{
    /// <summary>
    /// Resolve the repository version.
    /// </summary>
    /// <returns>The version string.</returns>
    string Resolve();
}
