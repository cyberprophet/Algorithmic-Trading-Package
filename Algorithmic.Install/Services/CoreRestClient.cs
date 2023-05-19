using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

using ShareInvest.Models;

using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareInvest.Services;

class CoreRestClient : RestClient
{
    internal async IAsyncEnumerable<FileVersionInfo> GetAsyncEnumerable(string appName)
    {
        var resource = string.Concat(App.Configuration.GetConnectionString(Properties.Resources.ROUTE),
                                     '/',
                                     Status.TransformQuery(JToken.FromObject(new
                                     {
                                         app = appName
                                     }),
                                     new StringBuilder(nameof(FileVersionInfo))));

        var res = await GetAsync(resource);

        if (res is string json)
        {
            var enumerable = JsonConvert.DeserializeObject<FileVersionInfo[]>(json);

            if (enumerable != null)
            {
                foreach (var obj in enumerable)

                    yield return obj;
            }
        }
    }
    internal async Task<object> GetAsync(string resource)
    {
        var res = await ExecuteAsync(new RestRequest(resource, Method.GET), cancellationTokenSource.Token);

        if (HttpStatusCode.OK != res.StatusCode)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(new
            {
                resource,
                res.StatusCode
            });
#endif
        }
        return res.Content;
    }
    internal CoreRestClient(string url) : base(url)
    {
        Timeout = -1;
        cancellationTokenSource = new CancellationTokenSource();
    }
    readonly CancellationTokenSource cancellationTokenSource;
}