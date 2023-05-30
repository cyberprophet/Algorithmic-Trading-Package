using Microsoft.ML;

using ShareInvest.Microsoft.Models;

namespace ShareInvest.Microsoft;

public abstract class MachineLearning : IDisposable
{
    public abstract void Evaluate(MLContext context, ITransformer model);

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public virtual void Learning(IEnumerable<Chart> enumerable)
    {
        IDataView dataView = context.Data.LoadFromEnumerable(enumerable);
        
        var columnPair = new[]
        {
            new InputOutputColumnPair(nameof(Chart.Start)),
            new InputOutputColumnPair(nameof(Chart.High)),
            new InputOutputColumnPair(nameof(Chart.Low)),
            new InputOutputColumnPair(nameof(Chart.Close)),
            new InputOutputColumnPair(nameof(Chart.Volume))
        };
        var normalize = context.Transforms.NormalizeMinMax(columnPair, fixZero: false);

        var normalizeTransform = normalize.Fit(dataView);

        var dataSplit = context.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var dataPrepEstimator = context.Transforms.Concatenate("Features", nameof(Chart.Start), nameof(Chart.High), nameof(Chart.Low), nameof(Chart.Close), nameof(Chart.Volume));

        var dataPrepTransformer = dataPrepEstimator.Fit(dataSplit.TrainSet);

        var transformedTrainingData = dataPrepTransformer.Transform(dataSplit.TrainSet);

        var estimator = context.Regression.Trainers.Sdca();

        var trainedModel = estimator.Fit(transformedTrainingData);

        var transformedTestData = dataPrepTransformer.Transform(dataSplit.TestSet);

        var testDataPredictions = trainedModel.Transform(transformedTestData);

        var metrics = context.Regression.Evaluate(testDataPredictions);

        System.Diagnostics.Debug.WriteLine(metrics);
    }
    public MachineLearning(int? seed = null)
    {
        context = seed != null ? new MLContext(seed) : new MLContext();
    }
    readonly MLContext context;
}