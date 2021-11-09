using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

[ExcludeFromCodeCoverage]
internal class ReleasableIssue
{
    public ReleasableIssue(int number, string title, IssueResolution resolution, IssueType type)
    {
        Number = number;
        Title = title;
        Resolution = resolution;
        Type = type;
    }

    public int Number { get; }

    public string Title { get; }

    public IssueResolution Resolution { get; }

    public IssueType Type { get; }
}
