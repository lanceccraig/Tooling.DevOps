using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Facts.Testing;
using LanceC.Tooling.DevOps.Properties;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Deploy.GitHub;

public class ReleaseServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly DeployOptions _options = new();

    private ReleaseService CreateSystemUnderTest(
        DeployOptions? options = default,
        StubConsole? stubConsole = default)
    {
        _mocker.Use(options ?? _options);
        _mocker.Use<IConsole>(stubConsole ?? new());
        return _mocker.CreateInstance<ReleaseService>();
    }

    public class TheCreateMethod : ReleaseServiceFacts
    {
        [Fact]
        public async Task BuildsReleaseBodyFromIssues()
        {
            // Arrange
            var version = "1.0.0";

            var mockRepositoryService = _mocker.GetMock<IRepositoryService>();

            var milestone = GitHubStubCreator.CreateMilestone(version);
            mockRepositoryService.Setup(repositoryService => repositoryService.GetMilestone(version))
                .ReturnsAsync(milestone);

            var issues = new[]
            {
                ReleasableIssueCreator
                    .Create("FooBar", number: 7, resolution: BuiltInIssueResolutions.Duplicate, type: BuiltInIssueTypes.Performance),
                ReleasableIssueCreator
                    .Create("Quuz", number: 6, resolution: BuiltInIssueResolutions.Completed, type: BuiltInIssueTypes.Discussion),
                ReleasableIssueCreator
                    .Create("Quux", number: 5, resolution: BuiltInIssueResolutions.Duplicate, type: BuiltInIssueTypes.Feature),
                ReleasableIssueCreator
                    .Create("Qux", number: 4, resolution: BuiltInIssueResolutions.Completed, type: BuiltInIssueTypes.Feature),
                ReleasableIssueCreator
                    .Create("Baz", number: 3, resolution: BuiltInIssueResolutions.Completed, type: BuiltInIssueTypes.Feature),
                ReleasableIssueCreator
                    .Create("Bar", number: 2, resolution: BuiltInIssueResolutions.ByDesignWontFix, type: BuiltInIssueTypes.Bug),
                ReleasableIssueCreator
                    .Create("Foo", number: 1, resolution: BuiltInIssueResolutions.Completed, type: BuiltInIssueTypes.Bug),
            };
            mockRepositoryService.Setup(repositoryService => repositoryService.GetIssues(milestone))
                .ReturnsAsync(issues);

            var expectedBody = new StringBuilder()
                .AppendLine("### Bugs")
                .AppendLine("- Foo (#1)")
                .AppendLine()
                .AppendLine("### Features")
                .AppendLine("- Baz (#3)")
                .AppendLine("- Qux (#4)")
                .ToString();

            var sut = CreateSystemUnderTest();

            // Act
            await sut.Create(version);

            // Assert
            mockRepositoryService.Verify(repositoryService => repositoryService.CreateRelease(version, expectedBody));
        }

        [Fact]
        public async Task ClosesMilestone()
        {
            // Arrange
            var version = "1.0.0";

            var mockRepositoryService = _mocker.GetMock<IRepositoryService>();

            var milestone = GitHubStubCreator.CreateMilestone(version);
            mockRepositoryService.Setup(repositoryService => repositoryService.GetMilestone(version))
                .ReturnsAsync(milestone);

            var issue = ReleasableIssueCreator.Create("Foo");
            mockRepositoryService.Setup(repositoryService => repositoryService.GetIssues(milestone))
                .ReturnsAsync(new[] { issue, });

            var sut = CreateSystemUnderTest();

            // Act
            await sut.Create(version);

            // Assert
            mockRepositoryService.Verify(repositoryService => repositoryService.CloseMilestone(milestone));
        }

        [Fact]
        public async Task UploadsConfiguredReleaseAssets()
        {
            // Arrange
            var version = "1.0.0";

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>()
                {
                    [@"C:\Repository\artifacts\Bar.nupkg"] = new MockFileData("bar"),
                    [@"C:\Repository\artifacts\Baz\Qux.exe"] = new MockFileData("baz"),
                },
                @"C:\Repository");
            _mocker.Use<IFileSystem>(stubFileSystem);

            var options = new DeployOptions();
            options.ReleaseAssetRelativePaths.Add("Bar.nupkg");
            options.ReleaseAssetRelativePaths.Add(@"Baz\Qux.exe");

            var mockRepositoryService = _mocker.GetMock<IRepositoryService>();

            var milestone = GitHubStubCreator.CreateMilestone(version);
            mockRepositoryService.Setup(repositoryService => repositoryService.GetMilestone(version))
                .ReturnsAsync(milestone);

            var issue = ReleasableIssueCreator.Create("Foo");
            mockRepositoryService.Setup(repositoryService => repositoryService.GetIssues(milestone))
                .ReturnsAsync(new[] { issue, });

            var release = GitHubStubCreator.CreateRelease();
            mockRepositoryService.Setup(repositoryService => repositoryService.CreateRelease(version, It.IsAny<string>()))
                .ReturnsAsync(release);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            await sut.Create(version);

            // Assert
            mockRepositoryService
                .Verify(repositoryService => repositoryService.CreateReleaseAsset(release, "Bar.nupkg", It.IsAny<Stream>()));
            mockRepositoryService
                .Verify(repositoryService => repositoryService.CreateReleaseAsset(release, "Qux.exe", It.IsAny<Stream>()));
        }

        [Fact]
        public async Task ReplacesVersionTokenWithActualVersionInAssetFilePaths()
        {
            // Arrange
            var version = "1.0.0";

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>()
                {
                    [@"C:\Repository\artifacts\Bar.1.0.0.nupkg"] = new MockFileData("bar"),
                    [@"C:\Repository\artifacts\Baz\Qux-1.0.0.exe"] = new MockFileData("baz"),
                },
                @"C:\Repository");
            _mocker.Use<IFileSystem>(stubFileSystem);

            var options = new DeployOptions();
            options.ReleaseAssetRelativePaths.Add("Bar.{version}.nupkg");
            options.ReleaseAssetRelativePaths.Add(@"Baz\Qux-{version}.exe");

            var mockRepositoryService = _mocker.GetMock<IRepositoryService>();

            var milestone = GitHubStubCreator.CreateMilestone(version);
            mockRepositoryService.Setup(repositoryService => repositoryService.GetMilestone(version))
                .ReturnsAsync(milestone);

            var issue = ReleasableIssueCreator.Create("Foo");
            mockRepositoryService.Setup(repositoryService => repositoryService.GetIssues(milestone))
                .ReturnsAsync(new[] { issue, });

            var release = GitHubStubCreator.CreateRelease();
            mockRepositoryService.Setup(repositoryService => repositoryService.CreateRelease(version, It.IsAny<string>()))
                .ReturnsAsync(release);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            await sut.Create(version);

            // Assert
            mockRepositoryService
                .Verify(repositoryService => repositoryService.CreateReleaseAsset(release, "Bar.1.0.0.nupkg", It.IsAny<Stream>()));
            mockRepositoryService
                .Verify(repositoryService => repositoryService.CreateReleaseAsset(release, "Qux-1.0.0.exe", It.IsAny<Stream>()));
        }

        [Fact]
        public async Task ExitsEarlyWhenNoReleasableIssuesFound()
        {
            // Arrange
            var version = "1.0.0";

            var stubConsole = new StubConsole();
            var mockRepositoryService = _mocker.GetMock<IRepositoryService>();

            var milestone = GitHubStubCreator.CreateMilestone(version);
            mockRepositoryService.Setup(repositoryService => repositoryService.GetMilestone(version))
                .ReturnsAsync(milestone);

            mockRepositoryService.Setup(repositoryService => repositoryService.GetIssues(milestone))
                .ReturnsAsync(Array.Empty<ReleasableIssue>());

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            await sut.Create(version);

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleaseStatusNoReleasableIssues));
            mockRepositoryService.Verify(
                repositoryService => repositoryService.CreateRelease(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task WritesErrorMessageWhenRepositoryServiceThrowsInvalidOperationException()
        {
            // Arrange
            var version = "1.0.0";

            var stubConsole = new StubConsole();
            var mockRepositoryService = _mocker.GetMock<IRepositoryService>();

            var milestone = GitHubStubCreator.CreateMilestone(version);
            mockRepositoryService.Setup(repositoryService => repositoryService.GetMilestone(version))
                .ReturnsAsync(milestone);

            var exception = new InvalidOperationException("Foo");
            mockRepositoryService.Setup(repositoryService => repositoryService.GetIssues(milestone))
                .Throws(exception);

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            await sut.Create(version);

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleaseStatusErrorFormat(exception.Message)));
        }

        [Fact]
        public async Task WritesErrorMessageWhenRepositoryServiceThrowsKeyNotFoundException()
        {
            // Arrange
            var version = "1.0.0";

            var stubConsole = new StubConsole();

            var exception = new KeyNotFoundException("Foo");
            _mocker.GetMock<IRepositoryService>()
                .Setup(repositoryService => repositoryService.GetMilestone(version))
                .Throws(exception);

            var sut = CreateSystemUnderTest(stubConsole: stubConsole);

            // Act
            await sut.Create(version);

            // Assert
            stubConsole.MockError.Verify(error => error.Write(Messages.ReleaseStatusErrorFormat(exception.Message)));
        }

        [Fact]
        public async Task ThrowsArgumentExceptionWhenVersionIsEmpty()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.Create(string.Empty));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenVersionIsNull()
        {
            // Arrange
            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.Create(default!));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}
