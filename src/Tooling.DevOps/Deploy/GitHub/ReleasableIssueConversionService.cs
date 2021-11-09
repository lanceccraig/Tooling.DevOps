using System.CommandLine;
using System.CommandLine.IO;
using Ardalis.GuardClauses;
using LanceC.Tooling.DevOps.Properties;
using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

internal class ReleasableIssueConversionService : IReleasableIssueConversionService
{
    private readonly DeployOptions _options;
    private readonly IConsole _console;

    public ReleasableIssueConversionService(DeployOptions options, IConsole console)
    {
        _options = options;
        _console = console;
    }

    public IReadOnlyCollection<ReleasableIssue> ConvertAll(IEnumerable<Issue> issues)
    {
        var releasableIssues = new List<ReleasableIssue>();
        if (issues is null || !issues.Any())
        {
            return releasableIssues;
        }

        var skippedCount = 0;
        foreach (var issue in issues)
        {
            var labelsByClassification = issue.Labels.GroupBy(label => GetClassificationFromLabelName(label.Name))
                .ToDictionary(labelGrouping => labelGrouping.Key, labelGrouping => labelGrouping.ToArray());

            labelsByClassification.TryGetValue(_options.IssueResolutionLabelClassification, out var resolutionLabels);
            var hasSingleResolutionLabel = EnsureSingleLabel(resolutionLabels, issue.Number, "resolution");

            labelsByClassification.TryGetValue(_options.IssueTypeLabelClassification, out var typeLabels);
            var hasSingleTypeLabel = EnsureSingleLabel(typeLabels, issue.Number, "type");

            if (!hasSingleResolutionLabel || !hasSingleTypeLabel)
            {
                skippedCount++;
                continue;
            }

            var resolutionLabel = resolutionLabels!.Single();
            var resolutionName = resolutionLabel.Name.Replace(
                _options.IssueResolutionLabelClassification.Prefix + _options.LabelPrefixSeparator,
                string.Empty,
                StringComparison.OrdinalIgnoreCase);
            var resolution = _options.IssueResolutions
                .SingleOrDefault(ir => ir.Name.Equals(resolutionName, StringComparison.OrdinalIgnoreCase));
            if (resolution is null)
            {
                _console.Error.WriteLine(Messages.ReleasableIssueResolutionLabelInvalidFormat(issue.Number, resolutionName));
                skippedCount++;
                continue;
            }

            var typeLabel = typeLabels!.Single();
            var typeName = typeLabel.Name.Replace(
                _options.IssueTypeLabelClassification.Prefix + _options.LabelPrefixSeparator,
                string.Empty,
                StringComparison.OrdinalIgnoreCase);
            var type = _options.IssueTypes.SingleOrDefault(it => it.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            if (type is null)
            {
                _console.Error.WriteLine(Messages.ReleasableIssueTypeLabelInvalidFormat(issue.Number, typeName));
                skippedCount++;
                continue;
            }

            releasableIssues.Add(new ReleasableIssue(issue.Number, issue.Title, resolution, type));
        }

        if (skippedCount > 0)
        {
            throw new InvalidOperationException(Messages.ReleasableIssuesSkippedFormat(skippedCount));
        }

        return releasableIssues;
    }

    private LabelClassification GetClassificationFromLabelName(string labelName)
    {
        Guard.Against.NullOrEmpty(labelName, nameof(labelName));

        var colonIndex = labelName.IndexOf(_options.LabelPrefixSeparator, StringComparison.OrdinalIgnoreCase);
        if (colonIndex == -1)
        {
            return _options.DefaultLabelClassification;
        }

        var labelPrefix = labelName[..colonIndex];
        var matchingLabelClassification = _options.LabelClassifications
            .SingleOrDefault(lc => lc.Prefix.Equals(labelPrefix, StringComparison.OrdinalIgnoreCase));
        if (matchingLabelClassification is null && _options.FailOnUnconfiguredLabelPrefixes)
        {
            throw new KeyNotFoundException(Messages.ReleasableIssueLabelPrefixInvalidFormat(labelPrefix));
        }

        return matchingLabelClassification ?? _options.DefaultLabelClassification;
    }

    private bool EnsureSingleLabel(IReadOnlyCollection<Label>? labels, int issueNumber, string classificationDisplayName)
    {
        if (labels is null || !labels.Any())
        {
            _console.Error.WriteLine(Messages.ReleasableIssueLabelMissingFormat(issueNumber, classificationDisplayName));
            return false;
        }

        if (labels.Count > 1)
        {
            _console.Error.WriteLine(Messages.ReleasableIssueLabelDuplicateFormat(issueNumber, classificationDisplayName));
            return false;
        }

        return true;
    }
}
