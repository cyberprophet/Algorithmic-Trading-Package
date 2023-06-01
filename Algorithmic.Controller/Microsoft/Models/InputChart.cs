using Microsoft.ML.Data;

namespace ShareInvest.Microsoft.Models;

public class InputChart
{
    [LoadColumn(0)]
    public string? Date
    {
        get; set;
    }
    /*
    [LoadColumn(1)]
    public float Start
    {
        get; set;
    }
    [LoadColumn(2)]
    public float High
    {
        get; set;
    }
    [LoadColumn(3)]
    public float Low
    {
        get; set;
    }
    [LoadColumn(5)]
    public float Volume
    {
        get; set;
    }
    */
    [LoadColumn(4)]
    public float Close
    {
        get; set;
    }
}