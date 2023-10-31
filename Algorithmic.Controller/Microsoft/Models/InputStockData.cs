using Microsoft.ML.Data;

namespace ShareInvest.Microsoft.Models;

public class InputStockData
{
    [LoadColumn(1)]
    public float[]? Open
    {
        get; set;
    }
    [LoadColumn(2)]
    public float[]? High
    {
        get; set;
    }
    [LoadColumn(3)]
    public float[]? Low
    {
        get; set;
    }
    [LoadColumn(4)]
    public float[]? Close
    {
        get; set;
    }
    [LoadColumn(5)]
    public float[]? Volume
    {
        get; set;
    }
    [LoadColumn(6), ColumnName("suitable")]
    public bool Suitable
    {
        get; set;
    }
}