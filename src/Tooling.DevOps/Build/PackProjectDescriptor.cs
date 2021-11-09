using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Build;

/// <summary>
/// Provides the descriptor of a project to be packed by the deploy process.
/// </summary>
[ExcludeFromCodeCoverage]
public class PackProjectDescriptor : ProjectDescriptorBase
{
    internal PackProjectDescriptor(string relativePath)
        : base(relativePath)
    {
    }
}
