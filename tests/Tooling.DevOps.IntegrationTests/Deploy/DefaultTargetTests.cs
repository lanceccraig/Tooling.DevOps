using LanceC.Tooling.DevOps.Deploy;
using LanceC.Tooling.DevOps.Deploy.GitHub;
using LanceC.Tooling.DevOps.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.Tooling.DevOps.IntegrationTests.Deploy;

public class DefaultTargetTests
{
    private readonly AutoMocker _mocker = new();
    private readonly RepositoryInfo _repositoryInfo = new("Name", "OwnerName");

    [Fact]
    public async Task RunsBuiltInTargets()
    {
        // Arrange
        var stubExecutor = _mocker.GetMock<IExecutor>();
        var mockReleaseService = _mocker.GetMock<IReleaseService>();
        var sut = CreateSystemUnderTest(
            stubExecutor,
            mockReleaseService,
            options => options.VersionResolutionStrategy = VersionResolutionStrategy.Assembly);

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockReleaseService.Verify(relaseService => relaseService.Create(It.IsAny<string>()));
    }

    [Fact]
    public async Task CompilesSpecifiedInstallerTargets()
    {
        // Arrange
        var firstInstallerPath = "build/Foo.iss";
        var secondInstallerPath = "build/Bar/Baz.iss";

        var mockExecutor = _mocker.GetMock<IExecutor>();
        var stubReleaseService = _mocker.GetMock<IReleaseService>();
        var sut = CreateSystemUnderTest(
            mockExecutor,
            stubReleaseService,
            options =>
            {
                options.VersionResolutionStrategy = VersionResolutionStrategy.Assembly;
                options.InstallerScriptRelativePaths.Add(firstInstallerPath);
                options.InstallerScriptRelativePaths.Add(secondInstallerPath);
            });

        // Act
        await sut.RunWithoutExitingAsync(Array.Empty<string>());

        // Assert
        mockExecutor.Verify(executor => executor.Run(
            "iscc",
            It.Is<string>(arguments => arguments.Contains($"{firstInstallerPath}\" /DVERSION=")),
            default));
        mockExecutor.Verify(executor => executor.Run(
            "iscc",
            It.Is<string>(arguments => arguments.Contains($"{secondInstallerPath}\" /DVERSION=")),
            default));
    }

    private Bullseye.Targets CreateSystemUnderTest(
        Mock<IExecutor> mockExecutor,
        Mock<IReleaseService> stubReleaseService,
        Action<DeployOptions>? optionsAction = default)
        => _mocker.Get<ServiceCollection>()
        .AddDeployTooling(_repositoryInfo, optionsAction)
        .Replace(new ServiceDescriptor(typeof(IExecutor), provider => mockExecutor.Object, ServiceLifetime.Singleton))
        .Replace(new ServiceDescriptor(typeof(IReleaseService), provider => stubReleaseService.Object, ServiceLifetime.Singleton))
        .BuildServiceProvider()
        .GetServices<ITarget>()
        .SetupAll();
}
