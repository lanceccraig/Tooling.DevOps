using Ardalis.GuardClauses;
using LanceC.Tooling.DevOps.Properties;
using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

internal class RepositoryService : IRepositoryService
{
    private readonly RepositoryInfo _repositoryInfo;
    private readonly IGitHubClientFactory _gitHubClientFactory;
    private readonly IReleasableIssueConversionService _releasableIssueConversionService;
    private IGitHubClient? _gitHubClient;

    public RepositoryService(
        RepositoryInfo repositoryInfo,
        IGitHubClientFactory gitHubClientFactory,
        IReleasableIssueConversionService releasableIssueConversionService)
    {
        _repositoryInfo = repositoryInfo;
        _gitHubClientFactory = gitHubClientFactory;
        _releasableIssueConversionService = releasableIssueConversionService;
    }

    private IGitHubClient Client
    {
        get
        {
            if (_gitHubClient is null)
            {
                _gitHubClient = _gitHubClientFactory.Create();
            }

            return _gitHubClient;
        }
    }

    public async Task CloseMilestone(Milestone milestone)
    {
        Guard.Against.Null(milestone, nameof(milestone));

        var milestoneUpdate = new MilestoneUpdate { State = ItemState.Closed, };

        await Client.Issue.Milestone.Update(_repositoryInfo.OwnerName, _repositoryInfo.Name, milestone.Number, milestoneUpdate)
            .ConfigureAwait(false);
    }

    public async Task<Milestone> GetMilestone(string title)
    {
        Guard.Against.NullOrEmpty(title, nameof(title));

        var milestones = await Client.Issue.Milestone.GetAllForRepository(_repositoryInfo.OwnerName, _repositoryInfo.Name)
            .ConfigureAwait(false);

        var milestone = milestones.SingleOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (milestone is null)
        {
            throw new KeyNotFoundException(Messages.MissingMilestoneFormat(title));
        }

        return milestone;
    }

    public async Task<IReadOnlyCollection<ReleasableIssue>> GetIssues(Milestone milestone)
    {
        Guard.Against.Null(milestone, nameof(milestone));

        var issueRequest = new RepositoryIssueRequest
        {
            Milestone = milestone.Number.ToString(),
            State = ItemStateFilter.All,
        };

        var issues = await Client.Issue.GetAllForRepository(_repositoryInfo.OwnerName, _repositoryInfo.Name, issueRequest)
            .ConfigureAwait(false);

        var releasableIssues = _releasableIssueConversionService.ConvertAll(issues);
        return releasableIssues;
    }

    public async Task<Release> CreateRelease(string name, string body)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.NullOrEmpty(body, nameof(body));

        var newRelease = new NewRelease(name)
        {
            Name = name,
            Body = body,
            Draft = true,
            Prerelease = name.Contains('-', StringComparison.OrdinalIgnoreCase),
        };

        var release = await Client.Repository.Release.Create(_repositoryInfo.OwnerName, _repositoryInfo.Name, newRelease)
            .ConfigureAwait(false);
        return release;
    }

    public async Task<ReleaseAsset> CreateReleaseAsset(Release release, string fileName, Stream stream)
    {
        Guard.Against.Null(release, nameof(release));
        Guard.Against.NullOrEmpty(fileName, nameof(fileName));
        Guard.Against.Null(stream, nameof(stream));

        var releaseAssetUpload = new ReleaseAssetUpload
        {
            FileName = fileName,
            ContentType = "application/octet-stream",
            RawData = stream,
            Timeout = TimeSpan.FromMinutes(5),
        };

        var releaseAsset = await Client.Repository.Release.UploadAsset(release, releaseAssetUpload)
            .ConfigureAwait(false);
        return releaseAsset;
    }
}
