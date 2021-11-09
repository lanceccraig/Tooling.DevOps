using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Build;

/// <summary>
/// Provides the descriptor of a project to be published by the build process.
/// </summary>
[ExcludeFromCodeCoverage]
public class PublishProjectDescriptor : ProjectDescriptorBase
{
    internal PublishProjectDescriptor(string relativePath, string profileName)
        : base(relativePath)
    {
        ProfileName = profileName;
    }

    /// <summary>
    /// Gets the name of the publish profile.
    /// </summary>
    public string ProfileName { get; }
}
