using LanceC.Tooling.DevOps.Internal;

namespace LanceC.Tooling.DevOps.Build.Targets;

internal abstract class TargetBase : ITarget
{
    protected TargetBase(IExecutor executor, BuildOptions options)
    {
        Executor = executor;
        Options = options;
    }

    public abstract string Name { get; }

    public virtual bool PreCondition => true;

    protected IExecutor Executor { get; }

    protected BuildOptions Options { get; }

    public abstract void Setup(Bullseye.Targets targets);
}
