namespace LanceC.Tooling.DevOps.Deploy.Versioning;

internal class VersionService : IVersionService
{
    private readonly DeployOptions _options;
    private readonly IEnumerable<IVersionResolutionStrategy> _resolutionStrategies;

    public VersionService(DeployOptions options, IEnumerable<IVersionResolutionStrategy> resolutionStrategies)
    {
        _options = options;
        _resolutionStrategies = resolutionStrategies;
    }

    public string Resolve()
    {
        var strategy = _resolutionStrategies.Single(s => s.Kind == _options.VersionResolutionStrategy);
        return strategy.Resolve();
    }
}
