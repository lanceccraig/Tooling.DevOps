namespace LanceC.Tooling.DevOps;

/// <summary>
/// Provides the test suites that are recognized by default.
/// </summary>
public static class BuiltInTestSuites
{
    /// <summary>
    /// Specifies tests that verify functionality for singular units.
    /// </summary>
    public const string Unit = "Unit";

    /// <summary>
    /// Specifies tests that verify functionality for integrated systems.
    /// </summary>
    public const string Integration = "Integration";

    /// <summary>
    /// Specifies tests that verify functionality for the full process of user-facing operations.
    /// </summary>
    public const string Functional = "Functional";
}
