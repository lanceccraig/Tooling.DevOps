using LanceC.Tooling.DevOps.IntegrationTests.Testing;
using LanceC.Tooling.DevOps.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;
using static LanceC.Tooling.DevOps.IntegrationTests.Testing.CommandLineUtility;

namespace LanceC.Tooling.DevOps.IntegrationTests.Build;

public class DefaultTargetTests
{
    private readonly AutoMocker _mocker = new();

    [Fact]
    public async Task RunsBuiltInTargets()
    {
        // Arrange
        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sequenceVerifier = new SequenceVerifier();

        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run(
                "powershell",
                "-Command New-Item -Path artifacts -ItemType Directory -Force | Out-Null",
                default))
            .Callback(sequenceVerifier.ExecuteCallback());
        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("powershell", "-Command Remove-Item *", "artifacts"))
            .Callback(sequenceVerifier.ExecuteCallback());
        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments("clean")), default))
            .Callback(sequenceVerifier.ExecuteCallback());
        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default))
            .Callback(sequenceVerifier.ExecuteCallback());

        var sut = CreateSystemUnderTest(mockExecutor);

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor.VerifyAll();
        sequenceVerifier.VerifyAll();
    }

    [Fact]
    public async Task DoesNotRunUnconfiguredTargets()
    {
        // Arrange
        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(mockExecutor);

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor.Verify(
            executor => executor.Run("dotnet", It.Is<string>(arguments => arguments.StartsWith("test ")), default),
            Times.Never);
        mockExecutor.Verify(
            executor => executor.Run("dotnet", It.Is<string>(arguments => arguments.StartsWith("pack ")), default),
            Times.Never);
        mockExecutor.Verify(
            executor => executor.Run("dotnet", It.Is<string>(arguments => arguments.StartsWith("publish ")), default),
            Times.Never);
    }

    [Fact]
    public async Task RunsSpecifiedTestTargets()
    {
        // Arrange
        var firstProjectPath = "tests/Foo/Foo.csproj";
        var secondProjectPath = "tests/Bar/Bar.csproj";
        var thirdProjectPath = "tests/Baz/Baz.csproj";
        var fourthProjectPath = "tests/Qux/Qux.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddTestProject(firstProjectPath, BuiltInTestSuites.Unit);
                options.AddTestProject(secondProjectPath, BuiltInTestSuites.Integration);
                options.AddTestProject(thirdProjectPath, BuiltInTestSuites.Functional);
                options.AddTestProject(fourthProjectPath, BuiltInTestSuites.Unit);
            });

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {firstProjectPath}")), default));
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {secondProjectPath}")), default));
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {thirdProjectPath}")), default));
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {fourthProjectPath}")), default));
    }

    [Fact]
    public async Task RunsSpecifiedPackTargets()
    {
        // Arrange
        var firstProjectPath = "src/Foo/Foo.csproj";
        var secondProjectPath = "src/Bar/Bar.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddPackProject(firstProjectPath);
                options.AddPackProject(secondProjectPath);
            });

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor
            .Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"pack {firstProjectPath} -o artifacts")), default));
        mockExecutor
            .Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"pack {secondProjectPath} -o artifacts")), default));
    }

    [Fact]
    public async Task RunsSpecifiedPublishTargets()
    {
        // Arrange
        var firstProjectPath = "src/Foo/Foo.csproj";
        var secondProjectPath = "src/Bar/Bar.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddPublishProject(firstProjectPath, "Artifacts");
                options.AddPublishProject(secondProjectPath, "Artifacts");
            });

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor.Verify(executor => executor.Run(
            "dotnet",
            It.Is(TrimArguments($"publish {firstProjectPath} -p:PublishProfile=Artifacts")),
            default));
        mockExecutor.Verify(executor => executor.Run(
            "dotnet",
            It.Is(TrimArguments($"publish {secondProjectPath} -p:PublishProfile=Artifacts")),
            default));
    }

    [Fact]
    public async Task RunsSpecifiedTestPackAndPublishTargets()
    {
        // Arrange
        var testProjectPath = "tests/Foo/Foo.csproj";
        var packProjectPath = "src/Bar/Bar.csproj";
        var publishProjectPath = "src/Baz/Baz.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddTestProject(testProjectPath, BuiltInTestSuites.Unit);
                options.AddPackProject(packProjectPath);
                options.AddPublishProject(publishProjectPath, "Artifacts");
            });

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {testProjectPath}")), default));
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"pack {packProjectPath} -o artifacts")), default));
        mockExecutor.Verify(executor => executor.Run(
            "dotnet",
            It.Is(TrimArguments($"publish {publishProjectPath} -p:PublishProfile=Artifacts")),
            default));
    }

    private Bullseye.Targets CreateSystemUnderTest(
        Mock<IExecutor> mockExecutor,
        Action<BuildOptions>? optionsAction = default)
        => _mocker.Get<ServiceCollection>()
        .AddBuildTooling(optionsAction)
        .Replace(new ServiceDescriptor(typeof(IExecutor), provider => mockExecutor.Object, ServiceLifetime.Singleton))
        .BuildServiceProvider()
        .GetServices<ITarget>()
        .SetupAll();
}
