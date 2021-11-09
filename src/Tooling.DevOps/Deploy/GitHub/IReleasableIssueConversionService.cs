using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Defines the service for converting GitHub issues into releasable issues.
/// </summary>
internal interface IReleasableIssueConversionService
{
    /// <summary>
    /// Converts the GitHub issues into releasable issues.
    /// </summary>
    /// <param name="issues">The GitHub issues to convert.</param>
    /// <returns>The releasable issues.</returns>
    IReadOnlyCollection<ReleasableIssue> ConvertAll(IEnumerable<Issue> issues);
}
