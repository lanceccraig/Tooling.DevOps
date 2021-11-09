using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Defines the service for interacting with a GitHub repository.
/// </summary>
internal interface IRepositoryService
{
    /// <summary>
    /// Closes a milestone.
    /// </summary>
    /// <param name="milestone">The milestone to close.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task CloseMilestone(Milestone milestone);

    /// <summary>
    /// Gets a milestone.
    /// </summary>
    /// <param name="title">The title of the milestone to retrieve.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation,
    /// containing the milestone.
    /// </returns>
    Task<Milestone> GetMilestone(string title);

    /// <summary>
    /// Gets issues for a milestone.
    /// </summary>
    /// <param name="milestone">The milestone to retrieve issues from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation,
    /// containing the releasable issues.
    /// </returns>
    Task<IReadOnlyCollection<ReleasableIssue>> GetIssues(Milestone milestone);

    /// <summary>
    /// Creates a release.
    /// </summary>
    /// <param name="name">The release name.</param>
    /// <param name="body">The release body.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation,
    /// containing the new release.
    /// </returns>
    Task<Release> CreateRelease(string name, string body);

    /// <summary>
    /// Creates a release asset.
    /// </summary>
    /// <param name="release">The release to create the asset in.</param>
    /// <param name="fileName">The asset file name.</param>
    /// <param name="stream">The asset data.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation,
    /// containing the new release asset.
    /// </returns>
    Task<ReleaseAsset> CreateReleaseAsset(Release release, string fileName, Stream stream);
}
