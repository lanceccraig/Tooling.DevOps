using Ardalis.GuardClauses;

namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides extensions for modifying build options.
/// </summary>
public static class BuildOptionsExtensions
{
    /// <summary>
    /// Adds a project to be tested by the build process.
    /// </summary>
    /// <param name="options">The build options to modify.</param>
    /// <param name="relativePath">The path of the project relative to the repository root.</param>
    /// <param name="testSuite">The suite of tests contained within the project.</param>
    /// <param name="forceBuild">
    /// The value that determines whether the project must be rebuilt before any additional executions.
    /// </param>
    public static void AddTestProject(this BuildOptions options, string relativePath, string testSuite, bool forceBuild = false)
    {
        Guard.Against.Null(options, nameof(options));

        options.TestProjects.Add(new Build.TestProjectDescriptor(relativePath, forceBuild, testSuite));
    }

    /// <summary>
    /// Adds a project to be packed by the build process.
    /// </summary>
    /// <param name="options">The build options to modify.</param>
    /// <param name="relativePath">The path of the project relative to the repository root.</param>
    /// <param name="forceBuild">
    /// The value that determines whether the project must be rebuilt before any additional executions.
    /// </param>
    public static void AddPackProject(this BuildOptions options, string relativePath, bool forceBuild = false)
    {
        Guard.Against.Null(options, nameof(options));

        options.PackProjects.Add(new Build.PackProjectDescriptor(relativePath, forceBuild));
    }

    /// <summary>
    /// Adds a project to be published by the build process.
    /// </summary>
    /// <param name="options">The build options to modify.</param>
    /// <param name="relativePath">The path of the project relative to the repository root.</param>
    /// <param name="profileName">The name of the publish profile.</param>
    /// <param name="forceBuild">
    /// The value that determines whether the project must be rebuilt before any additional executions.
    /// </param>
    public static void AddPublishProject(this BuildOptions options, string relativePath, string profileName, bool forceBuild = false)
    {
        Guard.Against.Null(options, nameof(options));

        options.PublishProjects.Add(new Build.PublishProjectDescriptor(relativePath, forceBuild, profileName));
    }
}
