using Google.Cloud.Speech.V1;

using System.Reflection;

namespace ShareInvest.Google;

public static class Cloud
{
    public static IEnumerable<string> GetLanguageCodes()
    {
        foreach (var property in typeof(LanguageCodes).GetProperties(BindingFlags.Static | BindingFlags.Public))
        {
            if (property.PropertyType.IsClass)
            {
                foreach (var p in property.GetType()
                                          .GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    yield return p.Name;
                }
                continue;
            }
            if (property.PropertyType == typeof(string))
            {
                yield return property.Name;
            }
        }
    }
}