using System.Linq.Expressions;

namespace LanceC.Tooling.DevOps.IntegrationTests.Testing;

public static class CommandLineUtility
{
    public static Expression<Func<string, bool>> TrimArguments(string expectedArguments)
        => actualArguments => actualArguments
        .Replace("-c Release", string.Empty)
        .Replace("-v minimal", string.Empty)
        .Replace("--no-build", string.Empty)
        .Replace("--nologo", string.Empty)
        .Trim() == expectedArguments;
}
