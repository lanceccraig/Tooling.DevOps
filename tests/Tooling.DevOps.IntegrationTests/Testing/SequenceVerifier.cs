using Moq;
using Xunit;

namespace LanceC.Tooling.DevOps.IntegrationTests.Testing;

public class SequenceVerifier
{
    private int _executionCount = 0;
    private int _setupCount = 0;

    public MockSequence Sequence { get; } = new MockSequence();

    public Action ExecuteCallback()
    {
        var callNumber = _setupCount++;
        return () => AssertCallNumber(callNumber);
    }

    public void VerifyAll() => Assert.Equal(_setupCount, _executionCount);

    private void AssertCallNumber(int expectedCallNumber)
    {
        Assert.Equal(expectedCallNumber, _executionCount);
        _executionCount++;
    }
}
