namespace ShareInvest.Microsoft.Models;

public class Chart
{
    public string? Date
    {
        get; set;
    }
    public int Close
    {
        get; set;
    }
    public int High
    {
        get; set;
    }
    public int Low
    {
        get; set;
    }
    public int Start
    {
        get; set;
    }
    public long Volume
    {
        get; set;
    }
}