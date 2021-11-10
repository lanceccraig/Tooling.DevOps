using LanceC.Tooling.DevOps.IntegrationTests.Testing;
using LanceC.Tooling.DevOps.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;
using static LanceC.Tooling.DevOps.IntegrationTests.Testing.CommandLineUtility;

namespace LanceC.Tooling.DevOps.IntegrationTests.Build;

public class SingleTargetTests
{
    private readonly AutoMocker _mocker = new();

    [Fact]
    public async Task RunsBuildTargetBeforeTestTargetWhenForceBuildIsNotSet()
    {
        // Arrange
        var projectPath = "tests/Foo/Foo.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sequenceVerifier = new SequenceVerifier();

        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default))
            .Callback(sequenceVerifier.ExecuteCallback());
        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {projectPath}")), default))
            .Callback(sequenceVerifier.ExecuteCallback());

        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddTestProject(projectPath, BuiltInTestSuites.Unit, forceBuild: false);
            });

        // Act
        await sut.RunWithoutExitingAsync(new[] { BuiltInBuildTargets.TestUnit, });

        // Assert
        mockExecutor.VerifyAll();
        sequenceVerifier.VerifyAll();
    }

    [Fact]
    public async Task DoesNotRunBuildTargetBeforeTestTargetWhenForceBuildIsSet()
    {
        // Arrange
        var projectPath = "tests/Foo/Foo.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddTestProject(projectPath, BuiltInTestSuites.Unit, forceBuild: true);
            });

        // Act
        await sut.RunWithoutExitingAsync(new[] { BuiltInBuildTargets.TestUnit, });

        // Assert
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default), Times.Never);
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"test {projectPath}")), default));
    }

    [Fact]
    public async Task RunsBuildTargetBeforePackTargetWhenForceBuildIsNotSet()
    {
        // Arrange
        var projectPath = "tests/Foo/Foo.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sequenceVerifier = new SequenceVerifier();

        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default))
            .Callback(sequenceVerifier.ExecuteCallback());
        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments($"pack {projectPath} -o artifacts")), default))
            .Callback(sequenceVerifier.ExecuteCallback());

        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddPackProject(projectPath, forceBuild: false);
            });

        // Act
        await sut.RunWithoutExitingAsync(new[] { BuiltInBuildTargets.Pack, });

        // Assert
        mockExecutor.VerifyAll();
        sequenceVerifier.VerifyAll();
    }

    [Fact]
    public async Task DoesNotRunBuildTargetBeforePackTargetWhenForceBuildIsSet()
    {
        // Arrange
        var projectPath = "tests/Foo/Foo.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddPackProject(projectPath, forceBuild: true);
            });

        // Act
        await sut.RunWithoutExitingAsync(new[] { BuiltInBuildTargets.Pack, });

        // Assert
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default), Times.Never);
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments($"pack {projectPath} -o artifacts")), default));
    }

    [Fact]
    public async Task RunsBuildTargetBeforePublishTargetWhenForceBuildIsNotSet()
    {
        // Arrange
        var projectPath = "tests/Foo/Foo.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sequenceVerifier = new SequenceVerifier();

        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default))
            .Callback(sequenceVerifier.ExecuteCallback());
        mockExecutor.InSequence(sequenceVerifier.Sequence)
            .Setup(executor => executor.Run(
                "dotnet",
                It.Is(TrimArguments($"publish {projectPath} -p:PublishProfile=Artifacts")),
                default))
            .Callback(sequenceVerifier.ExecuteCallback());

        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddPublishProject(projectPath, "Artifacts", forceBuild: false);
            });

        // Act
        await sut.RunWithoutExitingAsync(new[] { BuiltInBuildTargets.Publish, });

        // Assert
        mockExecutor.VerifyAll();
        sequenceVerifier.VerifyAll();
    }

    [Fact]
    public async Task DoesNotRunBuildTargetBeforePublishTargetWhenForceBuildIsSet()
    {
        // Arrange
        var projectPath = "tests/Foo/Foo.csproj";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            options =>
            {
                options.AddPublishProject(projectPath, "Artifacts", forceBuild: true);
            });

        // Act
        await sut.RunWithoutExitingAsync(new[] { BuiltInBuildTargets.Publish, });

        // Assert
        mockExecutor.Verify(executor => executor.Run("dotnet", It.Is(TrimArguments("build")), default), Times.Never);
        mockExecutor.Verify(executor => executor.Run(
            "dotnet",
            It.Is(TrimArguments($"publish {projectPath} -p:PublishProfile=Artifacts")),
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
