using Microsoft.ML;

using ShareInvest.Microsoft.Models;

namespace ShareInvest.Microsoft.ML;

public class BinaryClassification : ModelBuilder
{
    public override dynamic Evaluate<T>(T data)
    {
        throw new NotImplementedException();
    }
    public override void Learning<T>(IEnumerable<T> enumerable)
    {
        IDataView dataView = context.Data.LoadFromEnumerable(enumerable);

        IEstimator<ITransformer> pipeline =

            context.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(InputStockData.Suitable));

        pipeline.Fit(dataView);
    }
    public BinaryClassification() : base()
    {

    }
}