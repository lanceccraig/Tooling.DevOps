using System.Diagnostics.CodeAnalysis;

namespace LanceC.Tooling.DevOps.Internal;

[ExcludeFromCodeCoverage]
internal class EnvironmentAccessor : IEnvironmentAccessor
{
    public string GetFolderPath(Environment.SpecialFolder folder)
        => Environment.GetFolderPath(folder);

    public string? GetVariable(string name, EnvironmentVariableTarget target)
        => Environment.GetEnvironmentVariable(name, target);
}
