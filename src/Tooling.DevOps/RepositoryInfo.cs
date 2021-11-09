namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides information about the GitHub repository.
/// </summary>
public class RepositoryInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryInfo"/> class.
    /// </summary>
    /// <param name="name">The name of the repository.</param>
    /// <param name="ownerName">The name of the owner.</param>
    public RepositoryInfo(string name, string ownerName)
    {
        Name = name;
        OwnerName = ownerName;
    }

    /// <summary>
    /// Gets the name of the repository.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the name of the owner.
    /// </summary>
    public string OwnerName { get; }
}
