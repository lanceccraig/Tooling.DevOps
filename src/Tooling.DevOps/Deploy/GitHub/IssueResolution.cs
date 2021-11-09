using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Provides the manner in which a GitHub issue was resolved.
/// </summary>
[ExcludeFromCodeCoverage]
public class IssueResolution
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IssueResolution"/> class.
    /// </summary>
    /// <param name="name">The name of the issue resolution.</param>
    /// <param name="inReleaseNotes">The value that determines whether the issue appears in release notes.</param>
    public IssueResolution(string name, bool inReleaseNotes)
    {
        Name = name;
        InReleaseNotes = inReleaseNotes;
    }

    /// <summary>
    /// Gets the name of the issue resolution.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value that determines whether the issue appears in release notes.
    /// </summary>
    public bool InReleaseNotes { get; }
}
