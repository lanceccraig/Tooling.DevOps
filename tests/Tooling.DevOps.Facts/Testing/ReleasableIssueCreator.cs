using LanceC.Tooling.DevOps.Deploy.GitHub;

namespace LanceC.Tooling.DevOps.Facts.Testing;

internal static class ReleasableIssueCreator
{
    private static int _counter;

    public static ReleasableIssue Create(
        string title,
        int? number = default,
        IssueResolution? resolution = default,
        IssueType? type = default)
        => new(
            number ?? ++_counter,
            title,
            resolution ?? BuiltInIssueResolutions.Completed,
            type ?? BuiltInIssueTypes.Feature);
}
