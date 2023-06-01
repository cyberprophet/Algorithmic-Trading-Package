namespace ShareInvest.Microsoft.Models;

public class OutputChart
{
    public float[]? ForecastedPrices
    {
        get; set;
    }
    public float[]? LowerBoundPrices
    {
        get; set;
    }
    public float[]? UpperBoundPrices
    {
        get; set;
    }
}