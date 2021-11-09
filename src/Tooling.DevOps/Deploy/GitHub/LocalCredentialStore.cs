using System.IO.Abstractions;
using LanceC.Tooling.DevOps.Internal;
using LanceC.Tooling.DevOps.Properties;
using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

internal class LocalCredentialStore : ICredentialStore
{
    private readonly DeployOptions _options;
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironmentAccessor _environment;

    public LocalCredentialStore(DeployOptions options, IFileSystem fileSystem, IEnvironmentAccessor environment)
    {
        _options = options;
        _fileSystem = fileSystem;
        _environment = environment;
    }

    public async Task<Credentials> GetCredentials()
    {
        var environmentVariableToken = !string.IsNullOrEmpty(_options.GitHubTokenEnvironmentVariableName)
            ? _environment.GetVariable(_options.GitHubTokenEnvironmentVariableName, EnvironmentVariableTarget.User)
            : default;
        if (!string.IsNullOrEmpty(environmentVariableToken))
        {
            return new Credentials(environmentVariableToken);
        }

        if (!string.IsNullOrEmpty(_options.GitHubTokenRelativePath))
        {
            var path = _fileSystem.Path.Combine(
                _environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                _options.GitHubTokenRelativePath);
            var fileToken = _fileSystem.File.Exists(path)
                ? await _fileSystem.File.ReadAllTextAsync(path)
                .ConfigureAwait(false)
                : default;
            if (!string.IsNullOrEmpty(fileToken))
            {
                return new Credentials(fileToken);
            }
        }

        throw new KeyNotFoundException(Messages.GitHubTokenMissing);
    }
}
