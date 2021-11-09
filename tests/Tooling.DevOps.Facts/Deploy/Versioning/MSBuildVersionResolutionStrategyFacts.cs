using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using LanceC.Tooling.DevOps.Deploy.Versioning;
using LanceC.Tooling.DevOps.Properties;
using Moq.AutoMock;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Deploy.Versioning;

public class MSBuildVersionResolutionStrategyFacts
{
    private readonly AutoMocker _mocker = new();

    private MSBuildVersionResolutionStrategy CreateSystemUnderTest()
        => _mocker.CreateInstance<MSBuildVersionResolutionStrategy>();

    public class TheResolveMethod : MSBuildVersionResolutionStrategyFacts
    {
        [Fact]
        public void ReturnsValueFromVersionElementInDirectoryBuildPropsFile()
        {
            // Arrange
            var expectedVersion = "1.0.1";

            var directoryBuildPropsContent = new StringBuilder()
                .AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">")
                .AppendLine("<ItemGroup>")
                .AppendLine("<PackageReference Include=\"Foo\" Version=\"1.0.0\" />")
                .AppendLine("</ItemGroup>")
                .AppendLine("<PropertyGroup>")
                .AppendLine("<GenerateDocumentationFile>true</GenerateDocumentationFile>")
                .AppendLine($"<Version>{expectedVersion}</Version>")
                .AppendLine("</PropertyGroup>")
                .AppendLine("</Project>")
                .ToString();

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Directory.Build.props"] = new MockFileData(directoryBuildPropsContent),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var actualVersion = sut.Resolve();

            // Assert
            Assert.Equal(expectedVersion, actualVersion);
        }

        [Fact]
        public void UsesFirstVersionElementValue()
        {
            // Arrange
            var expectedVersion = "1.0.1-preview.2";

            var directoryBuildPropsContent = new StringBuilder()
                .AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">")
                .AppendLine("<ItemGroup>")
                .AppendLine("<PackageReference Include=\"Foo\" Version=\"1.0.0\" />")
                .AppendLine("</ItemGroup>")
                .AppendLine("<PropertyGroup>")
                .AppendLine("<GenerateDocumentationFile>true</GenerateDocumentationFile>")
                .AppendLine($"<Version>{expectedVersion}</Version>")
                .AppendLine("<Version>1.0.1</Version>")
                .AppendLine("</PropertyGroup>")
                .AppendLine("</Project>")
                .ToString();

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Directory.Build.props"] = new MockFileData(directoryBuildPropsContent),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var actualVersion = sut.Resolve();

            // Assert
            Assert.Equal(expectedVersion, actualVersion);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionWhenDirectoryBuildPropsFileMissing()
        {
            // Arrange
            var stubFileSystem = new MockFileSystem();
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var exception = Record.Exception(() => sut.Resolve());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(Messages.DirectoryBuildFileMissing, exception.Message);
        }

        [Fact]
        public void ThrowsKeyNotFoundExceptionWhenVersionElementMissingFromDirectoryBuildPropsFile()
        {
            // Arrange
            var directoryBuildPropsContent = new StringBuilder()
                .AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">")
                .AppendLine("<ItemGroup>")
                .AppendLine("<PackageReference Include=\"Foo\" Version=\"1.0.0\" />")
                .AppendLine("</ItemGroup>")
                .AppendLine("<PropertyGroup>")
                .AppendLine("<GenerateDocumentationFile>true</GenerateDocumentationFile>")
                .AppendLine("</PropertyGroup>")
                .AppendLine("</Project>")
                .ToString();

            var stubFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    [@"C:\Directory.Build.props"] = new MockFileData(directoryBuildPropsContent),
                });
            _mocker.Use<IFileSystem>(stubFileSystem);

            var sut = CreateSystemUnderTest();

            // Act
            var exception = Record.Exception(() => sut.Resolve());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<KeyNotFoundException>(exception);
            Assert.Equal(Messages.DirectoryBuildVersionElementMissing, exception.Message);
        }
    }
}
