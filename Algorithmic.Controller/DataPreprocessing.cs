using ShareInvest.ML.Models;

namespace ShareInvest;

public class DataPreprocessing
{
    public IEnumerable<(string, ConditionData)> StartTestSetProcess()
    {
        for (int y = 0; y < 5; y++)
        {
            var inputCharts = new InputStockChart[120];

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (i >= list.Count - 120)
                {
                    inputCharts[inputCharts.Length - list.Count + i] = list[i - y];

                    continue;
                }
                break;
            }
            int maxPrice = inputCharts.Max(s => s.High), minPrice = inputCharts.Min(s => s.Low);
            long maxVolume = inputCharts.Max(s => s.Volume), minVolume = inputCharts.Min(s => s.Volume);
            string? maxDateTime = inputCharts.Max(s => s.DateTime);

            if (inputCharts[^1].Volume == 0 || string.IsNullOrEmpty(maxDateTime) ||
                maxPrice == minPrice && maxPrice == 0 || maxVolume == minVolume && maxVolume == 0)
            {
                continue;
            }
            var conditionData = new ConditionData
            {
                Satisfy = false,
                HighPrices = new float[inputCharts.Length],
                OpenPrices = new float[inputCharts.Length],
                LowPrices = new float[inputCharts.Length],
                ClosePrices = new float[inputCharts.Length],
                Volumes = new float[inputCharts.Length]
            };
            Normalization np = new(maxPrice, (float)minPrice), nv = new(maxVolume, (float)minVolume);

            for (int i = 0; i < inputCharts.Length; i++)
            {
                conditionData.ClosePrices[i] = np.Normalize(inputCharts[i].Current);
                conditionData.LowPrices[i] = np.Normalize(inputCharts[i].Low);
                conditionData.HighPrices[i] = np.Normalize(inputCharts[i].High);
                conditionData.OpenPrices[i] = np.Normalize(inputCharts[i].Start);
                conditionData.Volumes[i] = nv.Normalize(inputCharts[i].Volume);
            }
            yield return (maxDateTime, conditionData);
        }
    }
    public IEnumerable<object> StartTrainSetProcess(double riseRate)
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
            long maxVolume = inputCharts.Max(s => s.Volume), minVolume = inputCharts.Min(s => s.Volume);

            list.RemoveAt(list.Count - 1);

            if (inputCharts[^1].Volume == 0 || maxPrice == minPrice && maxPrice == 0 || maxVolume == minVolume && maxVolume == 0)
            {
                continue;
            }
            var conditionData = new ConditionData
            {
                Satisfy = false,
                HighPrices = new float[inputCharts.Length],
                OpenPrices = new float[inputCharts.Length],
                LowPrices = new float[inputCharts.Length],
                ClosePrices = new float[inputCharts.Length],
                Volumes = new float[inputCharts.Length]
            };
            Normalization np = new(maxPrice, (float)minPrice), nv = new(maxVolume, (float)minVolume);

            for (int i = 0; i < forecastedCharts.Length; i++)
            {
                if (forecastedCharts[i].Current > inputCharts[^1].Current * riseRate)
                {
                    conditionData.Satisfy = true;

                    break;
                }
            }
            if (conditionData.Satisfy && Environment.ProcessorCount < 8 && inputCharts[^1].DateTime is string referenceDate)
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
            }
            yield return conditionData;
        }
    }
    public DataPreprocessing(List<InputStockChart> list)
    {
        this.list = list;
    }
    readonly List<InputStockChart> list;
}