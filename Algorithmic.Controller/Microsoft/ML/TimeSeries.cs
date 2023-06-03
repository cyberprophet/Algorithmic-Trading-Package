using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

using ShareInvest.Microsoft.Models;

namespace ShareInvest.Microsoft.ML;

public class TimeSeries : ModelBuilder
{
    public override void Evaluate(MLContext context, ITransformer model)
    {
        throw new NotImplementedException();
    }
    public override void Learning<T>(IEnumerable<T> enumerable) where T : class
    {
        IDataView dataView = context.Data.LoadFromEnumerable(enumerable);

        IEstimator<ITransformer> pipeline =

            context.Forecasting.ForecastBySsa(outputColumnName: nameof(OutputChart.ForecastedPrices),
                                              inputColumnName: nameof(InputChart.Close),
                                              windowSize: 5,
                                              seriesLength: 20,
                                              trainSize: 60,
                                              horizon: 2,
                                              confidenceLevel: 0.95f,
                                              confidenceLowerBoundColumn: nameof(OutputChart.LowerBoundPrices),
                                              confidenceUpperBoundColumn: nameof(OutputChart.UpperBoundPrices));

        ITransformer model = pipeline.Fit(dataView);

        var engine = model.CreateTimeSeriesEngine<InputChart, OutputChart>(context);
    }
    public TimeSeries(int? seed = null) : base(seed)
    {

    }
}