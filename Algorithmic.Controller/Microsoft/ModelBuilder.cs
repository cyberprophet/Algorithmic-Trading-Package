using Microsoft.ML;

namespace ShareInvest.Microsoft;

public abstract class ModelBuilder : IDisposable
{
    public abstract dynamic? Evaluate<T>(T data) where T : class;

    public abstract void Learning<T>(IEnumerable<T> enumerable) where T : class;

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public ModelBuilder(int? seed = null)
    {
        context = seed != null ? new MLContext(seed) : new MLContext();
    }
    protected internal readonly MLContext context;
}