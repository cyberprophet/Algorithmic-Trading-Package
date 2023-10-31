using Microsoft.ML.Data;

namespace ShareInvest.ML.Models;

public class ConditionPrediction : ConditionData
{
    [ColumnName("PredictedLabel")]
    public bool Prediction
    {
        get; set;
    }
    public float Probability
    {
        get; set;
    }
    public float Score
    {
        get; set;
    }
}