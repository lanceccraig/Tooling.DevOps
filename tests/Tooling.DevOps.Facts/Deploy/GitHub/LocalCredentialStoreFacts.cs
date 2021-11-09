using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Internal;
using LanceC.Tooling.DevOps.Properties;
using Moq.AutoMock;
using Octokit;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Deploy.GitHub;

public class LocalCredentialStoreFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly DeployOptions _options = new();

    private LocalCredentialStore CreateSystemUnderTest(DeployOptions? options = default)
    {
        _mocker.Use(options ?? _options);
        return _mocker.CreateInstance<LocalCredentialStore>();
    }

    public class TheGetCredentialsMethod : LocalCredentialStoreFacts
    {
        [Fact]
        public async Task ReturnsCredentialsFromEnvironmentVariable()
        {
            // Arrange
            var token = "foo";

            _mocker.GetMock<IEnvironmentAccessor>()
                .Setup(environment => environment.GetVariable(
                    _options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns(token);

            var sut = CreateSystemUnderTest();

            // Act
            var credentials = await sut.GetCredentials();

            // Assert
            Assert.Equal(token, credentials.Password);
            Assert.Equal(AuthenticationType.Oauth, credentials.AuthenticationType);
        }

        [Theory]
        [InlineData(@".github\token")]
        [InlineData(@".github\..\.github\token")]
        public async Task ReturnsCredentialsFromFile(string relativePath)
        {
            // Arrange
            var token = "foo";

            var options = new DeployOptions
            {
                GitHubTokenRelativePath = relativePath,
            };

            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Users\testhost\.github\token"] = new MockFileData(token),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var credentials = await sut.GetCredentials();

            // Assert
            Assert.Equal(token, credentials.Password);
            Assert.Equal(AuthenticationType.Oauth, credentials.AuthenticationType);
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        public async Task ReturnsCredentialsFromFileWhenEnvironmentVariableNotSpecified(string? environmentVariableName)
        {
            // Arrange
            var token = "foo";

            var options = new DeployOptions
            {
                GitHubTokenEnvironmentVariableName = environmentVariableName!,
            };

            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Users\testhost\.github\token"] = new MockFileData(token),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var credentials = await sut.GetCredentials();

            // Assert
            Assert.Equal(token, credentials.Password);
            Assert.Equal(AuthenticationType.Oauth, credentials.AuthenticationType);
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        public async Task ReturnsCredentialsFromFileWhenEnvironmentVariableNotFound(string? environmentVariableValue)
        {
            // Arrange
            var token = "foo";

            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment
                .Setup(environment => environment.GetVariable(
                    _options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns(environmentVariableValue);
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Users\testhost\.github\token"] = new MockFileData(token),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var credentials = await sut.GetCredentials();

            // Assert
            Assert.Equal(token, credentials.Password);
            Assert.Equal(AuthenticationType.Oauth, credentials.AuthenticationType);
        }

        [Fact]
        public async Task UsesEnvironmentVariableNameFromOptions()
        {
            // Arrange
            var token = "foo";

            var options = new DeployOptions
            {
                GitHubTokenEnvironmentVariableName = "FakeVariable",
            };

            _mocker.GetMock<IEnvironmentAccessor>()
                .Setup(environment => environment.GetVariable(
                    options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns(token);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var credentials = await sut.GetCredentials();

            // Assert
            Assert.Equal(token, credentials.Password);
            Assert.Equal(AuthenticationType.Oauth, credentials.AuthenticationType);
        }

        [Fact]
        public async Task UsesTokenRelativePathFromOptions()
        {
            // Arrange
            var token = "foo";

            var options = new DeployOptions
            {
                GitHubTokenRelativePath = @"FakeDirectory\FakeFile",
            };

            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment
                .Setup(environment => environment.GetVariable(
                    _options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns<string>(default);
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Users\testhost\FakeDirectory\FakeFile"] = new MockFileData(token),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var credentials = await sut.GetCredentials();

            // Assert
            Assert.Equal(token, credentials.Password);
            Assert.Equal(AuthenticationType.Oauth, credentials.AuthenticationType);
        }

        [Theory]
        [InlineData(default, default)]
        [InlineData("", "")]
        public async Task ThrowsKeyNotFoundExceptionWhenEnvironmentVariableEmptyAndFileNotSpecified(
            string? environmentVariableValue,
            string? relativePath)
        {
            // Arrange
            var options = new DeployOptions
            {
                GitHubTokenRelativePath = relativePath!,
            };

            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment
                .Setup(environment => environment.GetVariable(
                    _options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns(environmentVariableValue);
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Users\testhost\.github\token"] = new MockFileData(string.Empty),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest(options: options);

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetCredentials());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
            Assert.Equal(Messages.GitHubTokenMissing, exception.Message);
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        public async Task ThrowsKeyNotFoundExceptionWhenEnvironmentVariableAndFileEmpty(string? environmentVariableValue)
        {
            // Arrange
            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment
                .Setup(environment => environment.GetVariable(
                    _options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns(environmentVariableValue);
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Users\testhost\.github\token"] = new MockFileData(string.Empty),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetCredentials());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
            Assert.Equal(Messages.GitHubTokenMissing, exception.Message);
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        public async Task ThrowsKeyNotFoundExceptionWhenEnvironmentVariableAndFileNotFound(string? environmentVariableValue)
        {
            // Arrange
            var stubEnvironment = _mocker.GetMock<IEnvironmentAccessor>();
            stubEnvironment
                .Setup(environment => environment.GetVariable(
                    _options.GitHubTokenEnvironmentVariableName,
                    EnvironmentVariableTarget.User))
                .Returns(environmentVariableValue);
            stubEnvironment.Setup(environment => environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .Returns(@"C:\Users\testhost");

            var stubFileSystem = new MockFileSystem();
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var exception = await Record.ExceptionAsync(async () => await sut.GetCredentials());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
            Assert.Equal(Messages.GitHubTokenMissing, exception.Message);
        }
    }
}
