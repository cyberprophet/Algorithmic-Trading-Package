using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Naver.Models;

using System.Net;

namespace ShareInvest.Naver;

public class Papago : RestClient
{
    public async Task<TranslateSyntax?> TranslateAsync(string syntax)
    {
        var request = new RestRequest("v1/papago/n2mt", Method.Post);

        request.AddParameter("source", "ko");
        request.AddParameter("target", "en");
        request.AddParameter("text", syntax);

        var response = await ExecuteAsync(request, cts.Token);

        if (HttpStatusCode.OK == response.StatusCode && string.IsNullOrEmpty(response.Content) is false)
        {
            return JsonConvert.DeserializeObject<PapagoResponse>(response.Content).TranslateSyntax;
        }
        return null;
    }
    public Papago(string clientId, string clientSecret) : base("https://openapi.naver.com", configureDefaultHeaders: headers =>
    {
        headers.Add("X-Naver-Client-Id", clientId);
        headers.Add("X-Naver-Client-Secret", clientSecret);
    })
    {
        cts = new CancellationTokenSource();
    }
    readonly CancellationTokenSource cts;
}