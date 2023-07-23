using Microsoft.ML;
using Microsoft.ML.Trainers;

using Newtonsoft.Json;

using ShareInvest.ML.Models;

namespace ShareInvest.Microsoft.ML;

public class BinaryClassification : ModelBuilder
{
    public ConditionPrediction Prediction(ITransformer model, ConditionData data)
    {
        var engine = context.Model.CreatePredictionEngine<ConditionData, ConditionPrediction>(model);

        return engine.Predict(data);
    }
    public override TDst Prediction<TSrc, TDst>(ITransformer model, TSrc data) where TSrc : class where TDst : class
    {
        var engine = context.Model.CreatePredictionEngine<InputConditionData, ConditionPrediction>(model);

        return (engine.Predict((data as InputConditionData)!) as TDst)!;
    }
    /// <summary>
    /// The AreaUnderROCCurve metric is equal to the probability that the algorithm ranks a randomly chosen positive instance higher than a randomly chosen negative one
    /// (assuming 'positive' ranks higher than 'negative').
    ///
    /// The F1Score metric gets the model's F1 score.
    /// 
    /// The F1 score is the harmonic mean of precision and recall: 2 * precision * recall / (precision + recall).
    /// </summary>
    /// <param name="predictions">transformed data to evaluate the model and display accuracy statistics</param>
    public override void Evaluate(ITransformer model)
    {
        var predictions = model.Transform(TestSet);

        var metrics = context.BinaryClassification.Evaluate(predictions, labelColumnName: labelColumnName);

        Console.WriteLine(JsonConvert.SerializeObject(metrics, Formatting.Indented));
    }
    public override ITransformer Learning<T>(IEnumerable<T> enumerable)
    {
        IDataView dataView = context.Data.LoadFromEnumerable(enumerable as IEnumerable<ConditionData>);
        /*
        var estimator = context.Transforms.Conversion.ConvertType(nameof(InputConditionData.ClosePrices), outputKind: DataKind.Single)

            .Append(context.Transforms.Conversion.ConvertType(nameof(InputConditionData.HighPrices), outputKind: DataKind.Single))
            .Append(context.Transforms.Conversion.ConvertType(nameof(InputConditionData.LowPrices), outputKind: DataKind.Single))
            .Append(context.Transforms.Conversion.ConvertType(nameof(InputConditionData.OpenPrices), outputKind: DataKind.Single))
            .Append(context.Transforms.Conversion.ConvertType(nameof(InputConditionData.Volumes), outputKind: DataKind.Single))
            .Append(context.Transforms.Conversion.ConvertType(nameof(InputConditionData.DateTimes), outputKind: DataKind.Single))
            .Append(context.Transforms.Concatenate(featureColumnName, nameof(InputConditionData.ClosePrices),
                                                                      nameof(InputConditionData.HighPrices),
                                                                      nameof(InputConditionData.LowPrices),
                                                                      nameof(InputConditionData.OpenPrices),
                                                                      nameof(InputConditionData.Volumes),
                                                                      nameof(InputConditionData.DateTimes)));
        */
        var estimator = context.Transforms.Concatenate(featureColumnName, nameof(ConditionData.ClosePrices),
                                                                          nameof(ConditionData.HighPrices),
                                                                          nameof(ConditionData.LowPrices),
                                                                          nameof(ConditionData.OpenPrices),
                                                                          nameof(ConditionData.Volumes));

        var splitDataView = context.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var pipeline = estimator.Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(options));

        TestSet = splitDataView.TestSet;

        return pipeline.Fit(splitDataView.TrainSet);
    }
    public BinaryClassification(int? seed = null, int? gpuDeviceId = null) : base(seed, gpuDeviceId)
    {
        options = new SdcaLogisticRegressionBinaryTrainer.Options
        {
            MaximumNumberOfIterations =
#if DEBUG
                                        default
#else
                                        0x800
#endif
                                        ,
            LabelColumnName = labelColumnName,
            FeatureColumnName = featureColumnName
        };
    }
    IDataView? TestSet
    {
        get; set;
    }
    readonly SdcaLogisticRegressionBinaryTrainer.Options options;
}