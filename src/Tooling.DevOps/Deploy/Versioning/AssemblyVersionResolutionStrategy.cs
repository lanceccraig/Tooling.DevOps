using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using LanceC.Tooling.DevOps.Properties;

namespace LanceC.Tooling.DevOps.Deploy.Versioning;

[ExcludeFromCodeCoverage]
internal class AssemblyVersionResolutionStrategy : IVersionResolutionStrategy
{
    public VersionResolutionStrategy Kind { get; } = VersionResolutionStrategy.Assembly;

    public string Resolve()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly is null)
        {
            throw new InvalidOperationException(Messages.AssemblyMissing);
        }

        var assemblyName = assembly.GetName();
        var assemblyVersion = assemblyName.Version;
        if (assemblyVersion is null)
        {
            throw new InvalidOperationException(Messages.AssemblyVersionMissing);
        }

        var version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Revision}";
        return version;
    }
}
