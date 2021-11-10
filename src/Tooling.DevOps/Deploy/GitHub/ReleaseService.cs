using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using System.Text;
using Ardalis.GuardClauses;
using LanceC.Tooling.DevOps.Properties;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

internal class ReleaseService : IReleaseService
{
    private const string VersionReplacementToken = "{version}";

    private readonly DeployOptions _options;
    private readonly IRepositoryService _repositoryService;
    private readonly IFileSystem _fileSystem;
    private readonly IConsole _console;

    public ReleaseService(
        DeployOptions options,
        IRepositoryService repositoryService,
        IFileSystem fileSystem,
        IConsole console)
    {
        _options = options;
        _repositoryService = repositoryService;
        _fileSystem = fileSystem;
        _console = console;
    }

    public async Task Create(string version)
    {
        try
        {
            Guard.Against.NullOrEmpty(version, nameof(version));

            _console.Out.WriteLine(Messages.ReleaseStatusMilestoneRetrieval);
            var milestone = await _repositoryService.GetMilestone(version)
                .ConfigureAwait(false);

            _console.Out.WriteLine(Messages.ReleaseStatusIssuesRetrieval);
            var issues = await _repositoryService.GetIssues(milestone)
                .ConfigureAwait(false);
            if (!issues.Any())
            {
                _console.Error.WriteLine(Messages.ReleaseStatusNoReleasableIssues);
                return;
            }

            _console.Out.WriteLine(Messages.ReleaseStatusReleaseBodyBuild);
            var releaseBody = BuildReleaseBody(issues);

            _console.Out.WriteLine(Messages.ReleaseStatusReleaseCreation);
            var release = await _repositoryService.CreateRelease(version, releaseBody)
                .ConfigureAwait(false);

            if (_options.ReleaseAssetRelativePaths.Any())
            {
                _console.Out.WriteLine(Messages.ReleaseStatusAssetsUpload);
                var directory = _fileSystem.Directory.GetCurrentDirectory();
                foreach (var assetRelativePath in _options.ReleaseAssetRelativePaths)
                {
                    var assetActualRelativePath = assetRelativePath
                        .Replace(VersionReplacementToken, version, StringComparison.OrdinalIgnoreCase);
                    _console.Out.WriteLine(Messages.ReleaseStatusAssetUploadFormat(assetActualRelativePath));

                    var assetName = _fileSystem.Path.GetFileName(assetActualRelativePath);
                    var assetPath = _fileSystem.Path.Combine(directory, _options.ArtifactsDirectoryName, assetActualRelativePath);
                    using var assetStream = _fileSystem.File.OpenRead(assetPath);

                    await _repositoryService.CreateReleaseAsset(release, assetName, assetStream)
                        .ConfigureAwait(false);
                }
            }

            _console.Out.WriteLine(Messages.ReleaseStatusMilestoneClose);
            await _repositoryService.CloseMilestone(milestone)
                .ConfigureAwait(false);

            _console.Out.WriteLine(Messages.ReleaseStatusComplete);
        }
        catch (Exception e) when (
            e is InvalidOperationException ||
            e is KeyNotFoundException)
        {
            _console.Error.WriteLine(Messages.ReleaseStatusErrorFormat(e.Message));
        }
    }

    private static string BuildReleaseBody(IReadOnlyCollection<ReleasableIssue> releasableIssues)
    {
        Guard.Against.Null(releasableIssues, nameof(releasableIssues));

        var builder = new StringBuilder();

        var issueTypes = releasableIssues.Where(issue => issue.Resolution.InReleaseNotes)
            .Select(issue => issue.Type)
            .Distinct()
            .Where(it => it.InReleaseNotes)
            .OrderBy(it => it.DisplayName)
            .ToArray();
        for (var i = 0; i < issueTypes.Length; i++)
        {
            var issueType = issueTypes[i];
            var issues = releasableIssues.Where(i => i.Type.Name.Equals(issueType.Name, StringComparison.OrdinalIgnoreCase))
                .Where(i => i.Resolution.InReleaseNotes)
                .OrderBy(i => i.Number)
                .ToArray();

            builder.AppendLine($"### {issueType.DisplayName}");
            foreach (var issue in issues)
            {
                builder.AppendLine($"- {issue.Title} (#{issue.Number})");
            }

            if (i != issueTypes.Length - 1)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}
