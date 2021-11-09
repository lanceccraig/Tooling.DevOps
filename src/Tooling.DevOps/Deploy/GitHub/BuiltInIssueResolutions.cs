namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Provides the issue resolution labels that are recognized by default.
/// </summary>
public static class BuiltInIssueResolutions
{
    /// <summary>
    /// Specifies that the issue did not result in any code changes.
    /// </summary>
    public static readonly IssueResolution ByDesignWontFix = new("By Design / Won't Fix", false);

    /// <summary>
    /// Specifies that the issue has been completed and all code changes have been merged.
    /// </summary>
    public static readonly IssueResolution Completed = new("Completed", true);

    /// <summary>
    /// Specifies that the issue has already been described by another issue.
    /// </summary>
    public static readonly IssueResolution Duplicate = new("Duplicate", false);
}
