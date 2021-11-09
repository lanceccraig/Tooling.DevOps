using LanceC.Tooling.DevOps.Deploy;
using LanceC.Tooling.DevOps.Deploy.Versioning;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Deploy.Versioning;

public class VersionServiceFacts
{
    private readonly AutoMocker _mocker = new();

    private VersionService CreateSystemUnderTest()
        => _mocker.CreateInstance<VersionService>();

    public class TheResolveMethod : VersionServiceFacts
    {
        [Fact]
        public void ReturnsStrategyResolution()
        {
            // Arrange
            var expectedVersion = "1.2.3";

            var options = new DeployOptions
            {
                VersionResolutionStrategy = VersionResolutionStrategy.Assembly,
            };
            _mocker.Use(options);

            var expectedMockStrategy = new Mock<IVersionResolutionStrategy>();
            expectedMockStrategy.SetupGet(strategy => strategy.Kind)
                .Returns(options.VersionResolutionStrategy);
            expectedMockStrategy.Setup(strategy => strategy.Resolve())
                .Returns(expectedVersion);

            var notExpectedMockStrategy = new Mock<IVersionResolutionStrategy>();
            notExpectedMockStrategy.SetupGet(strategy => strategy.Kind)
                .Returns(VersionResolutionStrategy.MSBuild);
            notExpectedMockStrategy.Setup(strategy => strategy.Resolve())
                .Returns("1.0.0");

            _mocker.Use<IEnumerable<IVersionResolutionStrategy>>(
                new[]
                {
                    expectedMockStrategy.Object,
                    notExpectedMockStrategy.Object,
                });

            var sut = CreateSystemUnderTest();

            // Act
            var actualVersion = sut.Resolve();

            // Assert
            Assert.Equal(expectedVersion, actualVersion);
        }
    }
}
