using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Build;

/// <summary>
/// Provides the descriptor of a project to be tested by the build process.
/// </summary>
[ExcludeFromCodeCoverage]
public class TestProjectDescriptor : ProjectDescriptorBase
{
    internal TestProjectDescriptor(string relativePath, bool forceBuild, string testSuite)
        : base(relativePath, forceBuild)
    {
        TestSuite = testSuite;
    }

    /// <summary>
    /// Gets the suite of tests contained within the project.
    /// </summary>
    public string TestSuite { get; }
}
