using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Defines the factory for GitHub clients.
/// </summary>
internal interface IGitHubClientFactory
{
    /// <summary>
    /// Creates an authenticated GitHub client.
    /// </summary>
    /// <returns>The GitHub client.</returns>
    IGitHubClient Create();
}
