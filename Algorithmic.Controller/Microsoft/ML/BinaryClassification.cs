using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

using Newtonsoft.Json;

using ShareInvest.ML.Models;

namespace ShareInvest.Microsoft.ML;

public class BinaryClassification : ModelBuilder
{
    public override void Evaluate(IDataView predictions)
    {
        var metrics = context.BinaryClassification.Evaluate(predictions, labelColumnName: labelColumnName);

        Console.WriteLine(JsonConvert.SerializeObject(metrics, Formatting.Indented));
    }
    public override IDataView Learning<T>(IEnumerable<T> enumerable)
    {
        IDataView dataView = context.Data.LoadFromEnumerable(enumerable as IEnumerable<InputConditionData>);

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

        var splitDataView = context.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var pipeline = estimator.Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(options));

        return pipeline.Fit(splitDataView.TrainSet).Transform(splitDataView.TestSet);
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
    readonly SdcaLogisticRegressionBinaryTrainer.Options options;
}