using LanceC.Tooling.DevOps.Deploy;
using LanceC.Tooling.DevOps.Deploy.GitHub;

namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the options used to customize the deploy process.
/// </summary>
public class DeployOptions
{
    /// <summary>
    /// Gets or sets the directory name where build artifacts are stored. The default is "artifacts".
    /// </summary>
    public string ArtifactsDirectoryName { get; set; } = "artifacts";

    /// <summary>
    /// Gets or sets the default classification that is used for labels without a prefix.
    /// </summary>
    public LabelClassification DefaultLabelClassification { get; set; } = BuiltInLabelClassifications.Miscellaneous;

    /// <summary>
    /// Gets or sets the value that determines whether a deployment fails when an unrecognized label prefix is encountered.
    /// </summary>
    public bool FailOnUnconfiguredLabelPrefixes { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the environment variable containing the GitHub access token. The default is "GITHUB_TOKEN".
    /// </summary>
    public string GitHubTokenEnvironmentVariableName { get; set; } = "GITHUB_TOKEN";

    /// <summary>
    /// <para>
    ///     Gets or sets the relative path of the GitHub access token file. The default is ".github\token".
    /// </para>
    /// <para>
    ///     This path is relative to the user profile folder.
    /// </para>
    /// </summary>
    public string GitHubTokenRelativePath { get; set; } = @".github\token";

    /// <summary>
    /// <para>
    ///     Gets the relative paths for the InnoScript installer scripts to compile as part of a deployment.
    /// </para>
    /// <para>
    ///     These paths are relative to the root of the repository.
    /// </para>
    /// </summary>
    public IList<string> InstallerScriptRelativePaths { get; } = new List<string>();

    /// <summary>
    /// Gets or sets the classification that is used to identify resolution labels.
    /// </summary>
    public LabelClassification IssueResolutionLabelClassification { get; set; } = BuiltInLabelClassifications.Resolution;

    /// <summary>
    /// Gets the recognized resolution labels.
    /// </summary>
    public IList<IssueResolution> IssueResolutions { get; } =
        new List<IssueResolution>
        {
            BuiltInIssueResolutions.ByDesignWontFix,
            BuiltInIssueResolutions.Completed,
            BuiltInIssueResolutions.Duplicate,
        };

    /// <summary>
    /// Gets or sets the classification that is used to identify type labels.
    /// </summary>
    public LabelClassification IssueTypeLabelClassification { get; set; } = BuiltInLabelClassifications.Type;

    /// <summary>
    /// Gets the recognized type labels.
    /// </summary>
    public IList<IssueType> IssueTypes { get; } =
        new List<IssueType>
        {
            BuiltInIssueTypes.Bug,
            BuiltInIssueTypes.Discussion,
            BuiltInIssueTypes.Documentation,
            BuiltInIssueTypes.Feature,
            BuiltInIssueTypes.Meta,
            BuiltInIssueTypes.Performance,
            BuiltInIssueTypes.Security,
            BuiltInIssueTypes.TechnicalDebt,
        };

    /// <summary>
    /// Gets the recognized label classifications.
    /// </summary>
    public IList<LabelClassification> LabelClassifications { get; } =
        new List<LabelClassification>
        {
            BuiltInLabelClassifications.Miscellaneous,
            BuiltInLabelClassifications.Area,
            BuiltInLabelClassifications.Resolution,
            BuiltInLabelClassifications.Size,
            BuiltInLabelClassifications.Status,
            BuiltInLabelClassifications.Type,
        };

    /// <summary>
    /// Gets or sets the separator between the prefix and name of a label. The default is ": ".
    /// </summary>
    public string LabelPrefixSeparator { get; set; } = ": ";

    /// <summary>
    /// Gets or sets the name of the product to provide in GitHub API calls. The default is "Tool.Deploy".
    /// </summary>
    public string ProductName { get; set; } = "Tool.Deploy";

    /// <summary>
    /// <para>
    ///     Gets the relative paths of the assets to upload to the GitHub release as part of a deployment.
    /// </para>
    /// <para>
    ///     These paths are relative to the artifacts directory of the repository. Any instance of a "{version}" string will be
    ///     replaced with the current repository version on deployment.
    /// </para>
    /// </summary>
    public IList<string> ReleaseAssetRelativePaths { get; } = new List<string>();

    /// <summary>
    /// Gets or sets the strategy for resolving the repository version. The default is MSBuild.
    /// </summary>
    public VersionResolutionStrategy VersionResolutionStrategy { get; set; } = VersionResolutionStrategy.MSBuild;
}
