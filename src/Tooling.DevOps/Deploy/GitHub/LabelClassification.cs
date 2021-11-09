using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Provides a classification of labels for use on GitHub issues.
/// </summary>
[ExcludeFromCodeCoverage]
public class LabelClassification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelClassification"/> class.
    /// </summary>
    /// <param name="name">The label classification name.</param>
    /// <param name="prefix">The prefix that appears on the label.</param>
    public LabelClassification(string name, string prefix)
    {
        Name = name;
        Prefix = prefix;
    }

    /// <summary>
    /// Gets the label classification name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the prefix that appears on the label.
    /// </summary>
    public string Prefix { get; }
}
