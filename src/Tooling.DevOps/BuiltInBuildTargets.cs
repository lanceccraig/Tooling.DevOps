namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the targets that are built-in to the build process.
/// </summary>
public static class BuiltInBuildTargets
{
    /// <summary>
    /// Builds the solution.
    /// </summary>
    public const string Build = "Build";

    /// <summary>
    /// Cleans the solution.
    /// </summary>
    public const string Clean = "Clean";

    /// <summary>
    /// Deletes the contents of the artifacts directory.
    /// </summary>
    public const string DeleteArtifacts = "Delete Artifacts";

    /// <summary>
    /// Packs the configured projects.
    /// </summary>
    public const string Pack = "Pack";

    /// <summary>
    /// Publishes all configured projects.
    /// </summary>
    public const string Publish = "Publish";

    /// <summary>
    /// Runs functional tests.
    /// </summary>
    public const string TestFunctional = "Test (Functional)";

    /// <summary>
    /// Runs integration tests.
    /// </summary>
    public const string TestIntegration = "Test (Integration)";

    /// <summary>
    /// Runs unit tests.
    /// </summary>
    public const string TestUnit = "Test (Unit)";
}
