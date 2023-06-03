using Microsoft.ML.Data;

namespace ShareInvest.Microsoft.Models;

public class InputChart
{
    [LoadColumn(0)]
    public string? Date
    {
        get; set;
    }
    [LoadColumn(1)]
    public float Close
    {
        get; set;
    }
}