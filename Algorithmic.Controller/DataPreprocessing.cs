using ShareInvest.ML.Models;

namespace ShareInvest;

public class DataPreprocessing
{
    public event EventHandler<InputConditionData>? Send;

    public IEnumerable<string> StartProcess(double riseRate)
    {
        while (list.Count > 125)
        {
            var forecastedCharts = new InputStockChart[5];
            var inputCharts = new InputStockChart[120];

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (i >= list.Count - 5)
                {
                    forecastedCharts[forecastedCharts.Length - list.Count + i] = list[i];

                    continue;
                }
                if (i >= list.Count - 125)
                {
                    inputCharts[inputCharts.Length - list.Count + i + 5] = list[i];

                    continue;
                }
                break;
            }
            int maxPrice = inputCharts.Max(s => s.High), minPrice = inputCharts.Min(s => s.Low);
            string? maxDateTime = inputCharts.Max(s => s.DateTime), minDateTime = inputCharts.Min(s => s.DateTime);
            long maxVolume = inputCharts.Max(s => s.Volume), minVolume = inputCharts.Min(s => s.Volume);

            list.RemoveAt(list.Count - 1);

            if (inputCharts[^1].Volume == 0 ||
                maxPrice == minPrice && maxPrice == 0 || maxVolume == minVolume && maxVolume == 0 ||
                string.IsNullOrEmpty(maxDateTime) || string.IsNullOrEmpty(minDateTime))
            {
                continue;
            }
            var conditionData = new InputConditionData
            {
                Satisfy = false,
                HighPrices = new double[inputCharts.Length],
                OpenPrices = new double[inputCharts.Length],
                LowPrices = new double[inputCharts.Length],
                ClosePrices = new double[inputCharts.Length],
                DateTimes = new double[inputCharts.Length],
                Volumes = new double[inputCharts.Length]
            };
            Normalization np = new(maxPrice, (double)minPrice),
                          nv = new(maxVolume, (double)minVolume),
                          nt = new(Convert.ToInt32(maxDateTime), (double)Convert.ToInt32(minDateTime));

            for (int i = 0; i < forecastedCharts.Length; i++)
            {
                if (forecastedCharts[i].Current > inputCharts[^1].Current * riseRate)
                {
                    conditionData.Satisfy = true;

                    break;
                }
            }
            if (conditionData.Satisfy && inputCharts[^1].DateTime is string referenceDate)
            {
                yield return referenceDate;
            }
            for (int i = 0; i < inputCharts.Length; i++)
            {
                conditionData.ClosePrices[i] = np.Normalize(inputCharts[i].Current);
                conditionData.LowPrices[i] = np.Normalize(inputCharts[i].Low);
                conditionData.HighPrices[i] = np.Normalize(inputCharts[i].High);
                conditionData.OpenPrices[i] = np.Normalize(inputCharts[i].Start);
                conditionData.Volumes[i] = nv.Normalize(inputCharts[i].Volume);
                conditionData.DateTimes[i] = nt.Normalize(Convert.ToInt32(inputCharts[i].DateTime));
            }
            Send?.Invoke(this, conditionData);
        }
    }
    public DataPreprocessing(List<InputStockChart> list)
    {
        this.list = list;
    }
    readonly List<InputStockChart> list;
}