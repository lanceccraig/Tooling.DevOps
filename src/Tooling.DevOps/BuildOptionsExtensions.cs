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
    public static void AddTestProject(this BuildOptions options, string relativePath, string testSuite)
    {
        Guard.Against.Null(options, nameof(options));

        options.TestProjects.Add(new Build.TestProjectDescriptor(relativePath, testSuite));
    }

    /// <summary>
    /// Adds a project to be packed by the build process.
    /// </summary>
    /// <param name="options">The build options to modify.</param>
    /// <param name="relativePath">The path of the project relative to the repository root.</param>
    public static void AddPackProject(this BuildOptions options, string relativePath)
    {
        Guard.Against.Null(options, nameof(options));

        options.PackProjects.Add(new Build.PackProjectDescriptor(relativePath));
    }

    /// <summary>
    /// Adds a project to be published by the build process.
    /// </summary>
    /// <param name="options">The build options to modify.</param>
    /// <param name="relativePath">The path of the project relative to the repository root.</param>
    /// <param name="profileName">The name of the publish profile.</param>
    public static void AddPublishProject(this BuildOptions options, string relativePath, string profileName)
    {
        Guard.Against.Null(options, nameof(options));

        options.PublishProjects.Add(new Build.PublishProjectDescriptor(relativePath, profileName));
    }
}
