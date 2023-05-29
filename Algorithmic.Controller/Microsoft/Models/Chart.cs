using Microsoft.ML.Data;

namespace ShareInvest.Microsoft.Models;

public class Chart
{
    [LoadColumn(0)]
    public string? Date
    {
        get; set;
    }
    [LoadColumn(1)]
    public double Start
    {
        get; set;
    }
    [LoadColumn(2)]
    public double High
    {
        get; set;
    }
    [LoadColumn(3)]
    public double Low
    {
        get; set;
    }
    [LoadColumn(4)]
    public double Close
    {
        get; set;
    }
    [LoadColumn(5)]
    public double Volume
    {
        get; set;
    }
}