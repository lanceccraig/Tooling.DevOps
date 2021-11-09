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
    protected ProjectDescriptorBase(string relativePath)
    {
        RelativePath = relativePath;
    }

    /// <summary>
    /// Gets the path of the project relative to the repository root.
    /// </summary>
    public string RelativePath { get; }

    /// <inheritdoc/>
    public override string ToString()
        => RelativePath;
}
