using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Facts.Testing;
using Moq;
using Moq.AutoMock;
using Octokit;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Deploy.GitHub;

public class RepositoryServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly RepositoryInfo _repositoryInfo = new("RepositoryName", "RepositoryOwnerName");

    private RepositoryService CreateSystemUnderTest(Mock<IGitHubClient>? mockClient = default)
    {
        _mocker.Use(_repositoryInfo);

        _mocker.GetMock<IGitHubClientFactory>()
            .Setup(clientFactory => clientFactory.Create())
            .Returns((mockClient ?? _mocker.GetMock<IGitHubClient>()).Object);

        return _mocker.CreateInstance<RepositoryService>();
    }

    public class TheCloseMilestoneMethod : RepositoryServiceFacts
    {
        [Fact]
        public async Task UpdatesMilestone()
        {
            // Arrange
            var milestone = GitHubStubCreator.CreateMilestone("1.0.0");

            var stubClient = _mocker.GetMock<IGitHubClient>();

            var stubIssuesClient = _mocker.GetMock<IIssuesClient>();
            stubClient.SetupGet(client => client.Issue)
                .Returns(stubIssuesClient.Object);

            var mockMilestonesClient = _mocker.GetMock<IMilestonesClient>();
            stubIssuesClient.SetupGet(client => client.Milestone)
                .Returns(mockMilestonesClient.Object);

            var sut = CreateSystemUnderTest(mockClient: stubClient);

            // Act
            await sut.CloseMilestone(milestone);

            // Assert
            mockMilestonesClient.Verify(client => client.Update(
                _repositoryInfo.OwnerName,
                _repositoryInfo.Name,
                milestone.Number,
                It.Is<MilestoneUpdate>(milestoneUpdate => milestoneUpdate.State == ItemState.Closed)));
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenMilestoneIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.CloseMilestone(default!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }

    public class TheGetMilestoneMethod : RepositoryServiceFacts
    {
        [Fact]
        public async Task ReturnsMilestone()
        {
            // Arrange
            var title = "1.0.0";

            var mockClient = _mocker.GetMock<IGitHubClient>();

            var expectedMilestone = GitHubStubCreator.CreateMilestone(title);
            var notExpectedMilestone = GitHubStubCreator.CreateMilestone("1.0.1");
            mockClient.Setup(client => client.Issue.Milestone.GetAllForRepository(_repositoryInfo.OwnerName, _repositoryInfo.Name))
                .ReturnsAsync(new[] { expectedMilestone, notExpectedMilestone, });

            var sut = CreateSystemUnderTest(mockClient: mockClient);

            // Act
            var actualMilestone = await sut.GetMilestone(title);

            // Assert
            Assert.Equal(expectedMilestone, actualMilestone);
        }

        [Fact]
        public async Task ThrowsArgumentExceptionWhenTitleIsEmpty()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetMilestone(string.Empty));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenTitleIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetMilestone(default!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task ThrowsKeyNotFoundExceptionWhenNoMilestoneIsFoundForTitle()
        {
            // Arrange
            var stubClient = _mocker.GetMock<IGitHubClient>();

            var notExpectedMilestone = GitHubStubCreator.CreateMilestone("1.0.0");
            stubClient.Setup(client => client.Issue.Milestone.GetAllForRepository(_repositoryInfo.OwnerName, _repositoryInfo.Name))
                .ReturnsAsync(new[] { notExpectedMilestone, });

            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetMilestone("1.0.1"));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
        }
    }

    public class TheGetIssuesMethod : RepositoryServiceFacts
    {
        [Fact]
        public async Task ReturnsReleasableIssues()
        {
            // Arrange
            var milestone = GitHubStubCreator.CreateMilestone("1.0.0");

            var mockClient = _mocker.GetMock<IGitHubClient>();

            var issues = new[] { GitHubStubCreator.CreateIssue("Foo"), };
            mockClient
                .Setup(client => client.Issue.GetAllForRepository(
                    _repositoryInfo.OwnerName,
                    _repositoryInfo.Name,
                    It.Is<RepositoryIssueRequest>(issueRequest =>
                        issueRequest.Milestone == milestone.Number.ToString() &&
                        issueRequest.State == ItemStateFilter.All)))
                .ReturnsAsync(issues);

            var expectedReleasableIssue = ReleasableIssueCreator.Create("Foo");
            _mocker.GetMock<IReleasableIssueConversionService>()
                .Setup(releasableIssueConversionService => releasableIssueConversionService.ConvertAll(issues))
                .Returns(new[] { expectedReleasableIssue, });

            var sut = CreateSystemUnderTest(mockClient: mockClient);

            // Act
            var releasableIssues = await sut.GetIssues(milestone);

            // Assert
            var actualReleasableIssue = Assert.Single(releasableIssues);
            Assert.Equal(expectedReleasableIssue, actualReleasableIssue);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenMilestoneIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetIssues(default!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }

    public class TheCreateReleaseMethod : RepositoryServiceFacts
    {
        [Fact]
        public async Task ReturnsCreatedRelease()
        {
            // Arrange
            var name = "1.0.0";
            var body = "This is a test release.";

            var mockClient = _mocker.GetMock<IGitHubClient>();

            var expectedRelease = GitHubStubCreator.CreateRelease();
            mockClient
                .Setup(client => client.Repository.Release.Create(
                    _repositoryInfo.OwnerName,
                    _repositoryInfo.Name,
                    It.Is<NewRelease>(newRelease =>
                        newRelease.Name == name &&
                        newRelease.Body == body &&
                        newRelease.Draft &&
                        !newRelease.Prerelease)))
                .ReturnsAsync(expectedRelease);

            var sut = CreateSystemUnderTest(mockClient: mockClient);

            // Act
            var actualRelease = await sut.CreateRelease(name, body);

            // Assert
            Assert.Equal(expectedRelease, actualRelease);
        }

        [Fact]
        public async Task MarksAsPrereleaseWhenNameContainsHyphen()
        {
            // Arrange
            var name = "1.0.0-preview.1";
            var body = "This is a test release.";

            var mockClient = _mocker.GetMock<IGitHubClient>();

            var expectedRelease = GitHubStubCreator.CreateRelease();
            mockClient
                .Setup(client => client.Repository.Release.Create(
                    _repositoryInfo.OwnerName,
                    _repositoryInfo.Name,
                    It.Is<NewRelease>(newRelease =>
                        newRelease.Name == name &&
                        newRelease.Body == body &&
                        newRelease.Draft &&
                        newRelease.Prerelease)))
                .ReturnsAsync(expectedRelease);

            var sut = CreateSystemUnderTest(mockClient: mockClient);

            // Act
            var actualRelease = await sut.CreateRelease(name, body);

            // Assert
            Assert.Equal(expectedRelease, actualRelease);
        }

        [Fact]
        public async Task ThrowsArgumentExceptionWhenNameIsEmpty()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.CreateRelease(string.Empty, "body"));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenNameIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.CreateRelease(default!, "body"));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentExceptionWhenBodyIsEmpty()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.CreateRelease("name", string.Empty));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenBodyIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.CreateRelease("name", default!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }

    public class TheCreateReleaseAssetMethod : RepositoryServiceFacts
    {
        [Fact]
        public async Task ReturnsUploadedReleaseAsset()
        {
            // Arrange
            var release = GitHubStubCreator.CreateRelease();
            var fileName = "Foo.nupkg";
            var stream = Stream.Null;

            var mockClient = _mocker.GetMock<IGitHubClient>();

            var expectedReleaseAsset = GitHubStubCreator.CreateReleaseAsset();
            mockClient
                .Setup(client => client.Repository.Release.UploadAsset(
                    release,
                    It.Is<ReleaseAssetUpload>(releaseAssetUpload =>
                        releaseAssetUpload.FileName == fileName &&
                        releaseAssetUpload.ContentType == "application/octet-stream" &&
                        releaseAssetUpload.RawData == stream &
                        releaseAssetUpload.Timeout == TimeSpan.FromMinutes(5)),
                    default))
                .ReturnsAsync(expectedReleaseAsset);

            var sut = CreateSystemUnderTest();

            // Act
            var actualReleaseAsset = await sut.CreateReleaseAsset(release, fileName, stream);

            // Assert
            Assert.Equal(expectedReleaseAsset, actualReleaseAsset);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenReleaseIsNull()
        {
            // Arrange
            var release = GitHubStubCreator.CreateRelease();
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(
                async () => await sut.CreateReleaseAsset(default!, "fileName.exe", Stream.Null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentExceptionWhenFileNameIsEmpty()
        {
            // Arrange
            var release = GitHubStubCreator.CreateRelease();
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(
                async () => await sut.CreateReleaseAsset(release, string.Empty, Stream.Null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenFileNameIsNull()
        {
            // Arrange
            var release = GitHubStubCreator.CreateRelease();
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(
                async () => await sut.CreateReleaseAsset(release, default!, Stream.Null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenStreamIsNull()
        {
            // Arrange
            var release = GitHubStubCreator.CreateRelease();
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(
                async () => await sut.CreateReleaseAsset(release, "fileName.exe", default!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}
