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
    public override void Learning(IEnumerable<InputChart> enumerable)
    {
        IDataView dataView = context.Data.LoadFromEnumerable<InputChart>(enumerable);
        /*
        var columnPair = new[]
        {
            new InputOutputColumnPair(nameof(InputChart.Start)),
            new InputOutputColumnPair(nameof(InputChart.High)),
            new InputOutputColumnPair(nameof(InputChart.Low)),
            new InputOutputColumnPair(nameof(InputChart.Close)),
            new InputOutputColumnPair(nameof(InputChart.Volume))
        };
        var dataSplit = context.Data.TrainTestSplit(dataView, testFraction: 0.2);

        IDataView trainDataView = dataSplit.TrainSet;
        IDataView testDataView = dataSplit.TestSet;

        var concatenate = context.Transforms.Concatenate("Features",
                                                         nameof(InputChart.Start),
                                                         nameof(InputChart.High),
                                                         nameof(InputChart.Low),
                                                         nameof(InputChart.Close),
                                                         nameof(InputChart.Volume));

        var normalize = context.Transforms.NormalizeMinMax(columnPair, fixZero: false);
        */
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