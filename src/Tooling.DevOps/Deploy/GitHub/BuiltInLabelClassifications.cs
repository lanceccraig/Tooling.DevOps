namespace LanceC.Tooling.DevOps.Deploy.GitHub;

/// <summary>
/// Provides the label classifications that are recognized by default.
/// </summary>
public static class BuiltInLabelClassifications
{
    /// <summary>
    /// Specifies labels that do not fit into any other defined group.
    /// </summary>
    public static readonly LabelClassification Miscellaneous = new("Miscellaneous", string.Empty);

    /// <summary>
    /// Specifies labels that define the area of affected code.
    /// </summary>
    public static readonly LabelClassification Area = new("Area", "area");

    /// <summary>
    /// Specifies labels that define the end result of the issue.
    /// </summary>
    public static readonly LabelClassification Resolution = new("Resolution", "res");

    /// <summary>
    /// Specifies labels that define the estimated effort required for completion.
    /// </summary>
    public static readonly LabelClassification Size = new("Size", "size");

    /// <summary>
    /// Specifies labels that define the current state of the issue.
    /// </summary>
    public static readonly LabelClassification Status = new("Status", "status");

    /// <summary>
    /// Specifies labels that define the category of changes contained within the issue.
    /// </summary>
    public static readonly LabelClassification Type = new("Type", "type");
}
