using Octokit;

namespace LanceC.Tooling.DevOps.Facts.Testing;

public static class GitHubStubCreator
{
    private static int _counter = 0;

    public static Issue CreateIssue(string title, int? number = default, IReadOnlyList<Label>? labels = default)
    {
        labels ??= Array.Empty<Label>();
        return new(
            default,
            default,
            default,
            default,
            number ?? ++_counter,
            default,
            title,
            default,
            default,
            default,
            labels,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default);
    }

    public static Label CreateLabel(string name)
        => new(
            default,
            default,
            name,
            default,
            default,
            default,
            default);

    public static Label CreateResolutionLabel(string nameSuffix, DeployOptions options)
        => CreateLabel(options.IssueResolutionLabelClassification.Prefix + options.LabelPrefixSeparator + nameSuffix);

    public static Label CreateTypeLabel(string nameSuffix, DeployOptions options)
        => CreateLabel(options.IssueTypeLabelClassification.Prefix + options.LabelPrefixSeparator + nameSuffix);

    public static Milestone CreateMilestone(string title, int? number = default)
        => new(
            default,
            default,
            default,
            number ?? ++_counter,
            default,
            default,
            title,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default);

    public static Release CreateRelease()
        => new();

    public static ReleaseAsset CreateReleaseAsset()
        => new();
}
