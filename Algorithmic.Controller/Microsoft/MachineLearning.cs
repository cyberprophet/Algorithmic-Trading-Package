using Microsoft.ML;

namespace ShareInvest.Microsoft;

public abstract class MachineLearning : IDisposable
{
    public abstract void Evaluate(MLContext context, ITransformer model);

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public virtual void Learning<T>(IEnumerable<T> enumerable) where T : class
    {
        IDataView dataView = context.Data.LoadFromEnumerable(enumerable);


    }
    public MachineLearning(int? seed = null)
    {
        context = seed != null ? new MLContext(seed) : new MLContext();
    }
    readonly MLContext context;
}