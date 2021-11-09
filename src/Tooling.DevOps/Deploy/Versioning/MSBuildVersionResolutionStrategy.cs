using System.IO.Abstractions;
using System.Xml.Linq;
using LanceC.Tooling.DevOps.Properties;

namespace LanceC.Tooling.DevOps.Deploy.Versioning;

internal class MSBuildVersionResolutionStrategy : IVersionResolutionStrategy
{
    private readonly IFileSystem _fileSystem;

    public MSBuildVersionResolutionStrategy(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public VersionResolutionStrategy Kind { get; } = VersionResolutionStrategy.MSBuild;

    public string Resolve()
    {
        var currentDirectory = _fileSystem.Directory.GetCurrentDirectory();
        var directoryBuildPropsPath = _fileSystem.Path.Combine(currentDirectory, "Directory.Build.props");
        if (!_fileSystem.File.Exists(directoryBuildPropsPath))
        {
            throw new InvalidOperationException(Messages.DirectoryBuildFileMissing);
        }

        var directoryBuildPropsStream = _fileSystem.File.OpenRead(directoryBuildPropsPath);
        var projectElement = XElement.Load(directoryBuildPropsStream);

        var propertyGroupElements = projectElement.Descendants("PropertyGroup");
        foreach (var propertyGroupElement in propertyGroupElements)
        {
            var versionElement = propertyGroupElement.Element("Version");
            if (versionElement is not null)
            {
                return versionElement.Value;
            }
        }

        throw new KeyNotFoundException(Messages.DirectoryBuildVersionElementMissing);
    }
}
