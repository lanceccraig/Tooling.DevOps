namespace LanceC.Tooling.DevOps.Deploy;

/// <summary>
/// Determines the mechanism used for discovering the repository version.
/// </summary>
public enum VersionResolutionStrategy
{
    /// <summary>
    /// Specifies that the version will be extracted from the "Version" element in a root-level "Directory.Build.props" file.
    /// </summary>
    MSBuild,

    /// <summary>
    /// Specifies that the version will be extracted from the entry assembly.
    /// </summary>
    Assembly,
}
