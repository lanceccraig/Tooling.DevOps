using System.CommandLine;
using System.CommandLine.IO;
using Moq;

namespace LanceC.Tooling.DevOps.Facts.Testing;

public class StubConsole : IConsole
{
    public Mock<IStandardStreamWriter> MockOut { get; } = new();

    public IStandardStreamWriter Out => MockOut.Object;

    public Mock<IStandardStreamWriter> MockError { get; } = new();

    public IStandardStreamWriter Error => MockError.Object;

    public bool IsErrorRedirected { get; }

    public bool IsInputRedirected { get; }

    public bool IsOutputRedirected { get; }
}
