using LanceC.Tooling.DevOps.Internal;
using Xunit;

namespace LanceC.Tooling.DevOps.Facts.Internal;

public class TargetExtensionsFacts
{
    public class TheSetupAllMethod : TargetExtensionsFacts
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenTargetCollectionIsNull()
        {
            // Arrange
            var targets = default(IEnumerable<ITarget>);

            // Act
            var exception = Record.Exception(() => targets!.SetupAll());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}
