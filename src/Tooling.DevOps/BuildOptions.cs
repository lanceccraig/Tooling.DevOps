using LanceC.Tooling.DevOps.Build;

namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the options used to customize the build process.
/// </summary>
public class BuildOptions
{
    /// <summary>
    /// Gets or sets the directory name where build artifacts are stored. The default is "artifacts".
    /// </summary>
    public string ArtifactsDirectoryName { get; set; } = "artifacts";

    /// <summary>
    /// Gets the descriptors for projects to be tested.
    /// </summary>
    public IList<TestProjectDescriptor> TestProjects { get; } = new List<TestProjectDescriptor>();

    /// <summary>
    /// Gets the descriptors for projects to be packed as NuGet packages.
    /// </summary>
    public IList<PackProjectDescriptor> PackProjects { get; } = new List<PackProjectDescriptor>();

    /// <summary>
    /// Gets the descriptors for projects to be published.
    /// </summary>
    public IList<PublishProjectDescriptor> PublishProjects { get; } = new List<PublishProjectDescriptor>();
}
