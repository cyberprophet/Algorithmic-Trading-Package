using Microsoft.ML.Data;

namespace ShareInvest.ML.Models;

public class InputConditionData
{
    [LoadColumn(0), VectorType(120)]
    public double[]? DateTimes
    {
        get; set;
    }
    [LoadColumn(1), VectorType(120)]
    public double[]? OpenPrices
    {
        get; set;
    }
    [LoadColumn(2), VectorType(120)]
    public double[]? HighPrices
    {
        get; set;
    }
    [LoadColumn(3), VectorType(120)]
    public double[]? LowPrices
    {
        get; set;
    }
    [LoadColumn(4), VectorType(120)]
    public double[]? ClosePrices
    {
        get; set;
    }
    [LoadColumn(5), VectorType(120)]
    public double[]? Volumes
    {
        get; set;
    }
    [LoadColumn(6), ColumnName("Label")]
    public bool Satisfy
    {
        get; set;
    }
}