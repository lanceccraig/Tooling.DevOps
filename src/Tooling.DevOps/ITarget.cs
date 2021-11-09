namespace LanceC.Tooling.DevOps;

/// <summary>
/// Defines a pipeline target.
/// </summary>
public interface ITarget
{
    /// <summary>
    /// Gets the target name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the pre-condition required to setup the target.
    /// </summary>
    bool PreCondition { get; }

    /// <summary>
    /// Prepares the target for execution.
    /// </summary>
    /// <param name="targets">The target collection to set this target up in.</param>
    void Setup(Bullseye.Targets targets);
}
