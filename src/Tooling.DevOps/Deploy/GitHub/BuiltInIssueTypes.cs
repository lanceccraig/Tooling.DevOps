namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Provides the issue type labels that are recognized by default.
/// </summary>
public static class BuiltInIssueTypes
{
    /// <summary>
    /// Specifies that the issue describes a problem with existing functionality.
    /// </summary>
    public static readonly IssueType Bug = new("Bug", true, "Bugs");

    /// <summary>
    /// Specifies that the issue describes discussion around potential changes.
    /// </summary>
    public static readonly IssueType Discussion = new("Discussion", false);

    /// <summary>
    /// Specifies that the issue describes a new for new or updated documents.
    /// </summary>
    public static readonly IssueType Documentation = new("Documentation", true);

    /// <summary>
    /// Specifies that the issue describes functionality to be added or modified.
    /// </summary>
    public static readonly IssueType Feature = new("Feature", true, "Features");

    /// <summary>
    /// Specifies that the issue describes functionality that affects development processes.
    /// </summary>
    public static readonly IssueType Meta = new("Meta", true);

    /// <summary>
    /// Specifies that the issue describes an improvement to performance.
    /// </summary>
    public static readonly IssueType Performance = new("Performance", true);

    /// <summary>
    /// Specifies that the issue describes an improvement to security.
    /// </summary>
    public static readonly IssueType Security = new("Security", true);

    /// <summary>
    /// Specifies that the issue describes an improvement to code but does not affect functionality.
    /// </summary>
    public static readonly IssueType TechnicalDebt = new("Tech Debt", true, "TechnicalDebt");
}
