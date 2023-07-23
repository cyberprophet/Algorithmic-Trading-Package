using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

using ShareInvest.Microsoft.Models;

namespace ShareInvest.Microsoft.ML;

public class TimeSeries : ModelBuilder
{
    public override Result Prediction<T, Result>(ITransformer model, T data) where T : class
    {
        var engine = model.CreateTimeSeriesEngine<InputChart, OutputChart>(context);

        return (engine.Predict((data as InputChart)!) as Result)!;
    }
    public override void Evaluate(ITransformer model)
    {
        using (var file = File.OpenRead(path))
        {
            model = context.Model.Load(file, out DataViewSchema schema);
        }
    }
    public override ITransformer Learning<T>(IEnumerable<T> enumerable) where T : class
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

        engine.CheckPoint(context, path);

        return model;
    }
    public TimeSeries(string code, int? seed = null) : base(seed, null)
    {
        var path = Path.Combine(Environment.CurrentDirectory, nameof(Models), nameof(TimeSeries));

        var di = new DirectoryInfo(path);

        if (di.Exists is false)
        {
            di.Create();
        }
        this.path = Path.Combine(path, string.Concat(code, Properties.Resources.ZIP));
    }
    readonly string path;
}