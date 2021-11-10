namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the descriptor of a project to be used within a DevOps process.
/// </summary>
public abstract class ProjectDescriptorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectDescriptorBase"/> class.
    /// </summary>
    /// <param name="relativePath">The path of the project relative to the repository root.</param>
    /// <param name="forceBuild">
    /// The value that determines whether the project must be rebuilt before any additional executions.
    /// </param>
    protected ProjectDescriptorBase(string relativePath, bool forceBuild)
    {
        RelativePath = relativePath;
        ForceBuild = forceBuild;
    }

    /// <summary>
    /// Gets the path of the project relative to the repository root.
    /// </summary>
    public string RelativePath { get; }

    /// <summary>
    /// Gets the value that determines whether the project must be rebuilt before any additional executions.
    /// </summary>
    public bool ForceBuild { get; }

    /// <inheritdoc/>
    public override string ToString()
        => RelativePath;
}
