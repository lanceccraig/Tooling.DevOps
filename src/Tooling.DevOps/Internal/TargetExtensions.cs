using Ardalis.GuardClauses;

namespace LanceC.Tooling.DevOps.Internal;

internal static class TargetExtensions
{
    public static Bullseye.Targets SetupAll(this IEnumerable<ITarget> targets)
    {
        Guard.Against.Null(targets, nameof(targets));

        var targetCollection = new Bullseye.Targets();

        var validTargets = targets.Where(t => t.PreCondition)
            .ToArray();
        foreach (var target in validTargets)
        {
            target.Setup(targetCollection);
        }

        targetCollection.Add("default", validTargets.Select(t => t.Name));
        return targetCollection;
    }
}
