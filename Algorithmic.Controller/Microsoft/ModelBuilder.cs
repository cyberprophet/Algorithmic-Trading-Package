using Microsoft.ML;

using ShareInvest.Microsoft.Models;

namespace ShareInvest.Microsoft;

public abstract class ModelBuilder : IDisposable
{
    public abstract void Evaluate(MLContext context, ITransformer model);

    public abstract void Learning(IEnumerable<InputChart> enumerable);

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public ModelBuilder(int? seed = null)
    {
        context = seed != null ? new MLContext(seed) : new MLContext();
    }
    protected internal readonly MLContext context;
}