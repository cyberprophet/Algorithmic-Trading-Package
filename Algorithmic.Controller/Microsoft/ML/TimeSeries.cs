using Microsoft.ML;

namespace ShareInvest.Microsoft.ML;

public class TimeSeries : MachineLearning
{
    public override void Evaluate(MLContext context, ITransformer model)
    {
        throw new NotImplementedException();
    }
    public TimeSeries(int? seed = null) : base(seed)
    {

    }
}