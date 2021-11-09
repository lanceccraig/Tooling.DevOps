using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Provides the category of changes contained within a GitHub issue.
/// </summary>
[ExcludeFromCodeCoverage]
public class IssueType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IssueType"/> class.
    /// </summary>
    /// <param name="name">The name of the issue type.</param>
    /// <param name="inReleaseNotes">The value that determines whether the issue appears in release notes.</param>
    /// <param name="displayName">The display name of the issue type.</param>
    public IssueType(string name, bool inReleaseNotes, string? displayName = default)
    {
        Name = name;
        InReleaseNotes = inReleaseNotes;
        DisplayName = !string.IsNullOrEmpty(displayName) ? displayName : name;
    }

    /// <summary>
    /// Gets the name of the issue type.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value that determines whether the issue appears in release notes.
    /// </summary>
    public bool InReleaseNotes { get; }

    /// <summary>
    /// Gets the display name of the issue type.
    /// </summary>
    public string DisplayName { get; }
}
