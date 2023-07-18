using Microsoft.ML;

namespace ShareInvest.Microsoft;

public abstract class ModelBuilder : IDisposable
{
    public abstract void Evaluate(IDataView predictions);

    public abstract IDataView Learning<T>(IEnumerable<T> enumerable) where T : class;

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public ModelBuilder(int? seed, int? gpuDeviceId)
    {
        context = seed != null ? new MLContext(seed) : new MLContext();

        context.GpuDeviceId = gpuDeviceId;
    }
    protected internal readonly MLContext context;
    protected internal const string labelColumnName = "Label";
    protected internal const string featureColumnName = "Features";
}