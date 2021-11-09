using System.Diagnostics.CodeAnalysis;
using Octokit;

namespace LanceC.Tooling.DevOps.Deploy.GitHub;

[ExcludeFromCodeCoverage]
internal class GitHubClientFactory : IGitHubClientFactory
{
    private readonly DeployOptions _options;
    private readonly ICredentialStore _credentialStore;

    public GitHubClientFactory(DeployOptions options, ICredentialStore credentialStore)
    {
        _options = options;
        _credentialStore = credentialStore;
    }

    public IGitHubClient Create()
        => new GitHubClient(new ProductHeaderValue(_options.ProductName), _credentialStore);
}
