namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Defines the service for creating a GitHub release from a milestone.
/// </summary>
internal interface IReleaseService
{
    /// <summary>
    /// Creates a GitHub release for a milestone version.
    /// </summary>
    /// <param name="version">The milestone version.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task Create(string version);
}
