using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ShareInvest.Properties;

using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShareInvest.Services;

static class Status
{
    [SupportedOSPlatform("windows8.0")]
    internal static bool IsAdministrator
    {
        get
        {
            using (var windows = WindowsIdentity.GetCurrent())
            {
                return new WindowsPrincipal(windows).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
    internal static async Task<bool> StartProcess(string name)
    {
        var url = App.Configuration.GetConnectionString(Resources.LAUNCHER);

        if (string.IsNullOrEmpty(url))
        {
            return false;
        }
        var path = App.Configuration.GetConnectionString(nameof(Resources.PATH));
        var client = new CoreRestClient(url);

        await foreach (var item in client.GetAsyncEnumerable(name))
        {
            if (string.IsNullOrEmpty(item.FileName))
            {
                continue;
            }
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            var fullFileName = Resources.PUBLISH.Equals(item.Path) || string.IsNullOrEmpty(item.Path) ?

                               System.IO.Path.Combine(path, item.FileName) :

                               System.IO.Path.Combine(path, item.Path[8..], item.FileName);

            if (new System.IO.FileInfo(fullFileName).Exists)
            {
                var vInfo = FileVersionInfo.GetVersionInfo(fullFileName);

                if (item.FileVersion?.Length > 0 && item.FileVersion.Equals(vInfo.FileVersion))
                {
                    continue;
                }
            }
            var model = Convert.ToString(await client.GetAsync(string.Concat(App.Configuration.GetConnectionString(Resources.ROUTE),
                                                                             '/',
                                                                             TransformQuery(JToken.FromObject(new
                                                                             {
                                                                                 app = item.App,
                                                                                 path = Resources.PUBLISH.Equals(item.Path) ? null : item.Path?[8..],
                                                                                 name = item.FileName
                                                                             }),
                                                                             new StringBuilder(nameof(FileVersionInfo))))));
            if (string.IsNullOrEmpty(model))
            {
                continue;
            }
            item.File = JsonConvert.DeserializeObject<Models.FileVersionInfo>(model)?.File;

            if (item.File != null)
            {
                await new File(fullFileName).WriteAllBytesAsync(item.File);
            }
        }
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = string.Concat(name, App.Configuration.GetConnectionString(Resources.EXECUTE)),
                WorkingDirectory = path,
                Verb = App.Configuration.GetConnectionString(Resources.ADMIN)
            }
        })
            return process.Start();
    }
    internal static string TransformQuery(JToken token, StringBuilder query)
    {
        query.Append('?');

        foreach (var j in token.Children<JProperty>())

            if (JTokenType.Null != j.Value.Type)
            {
                query.Append(j.Path);
                query.Append('=');
                query.Append(j.Value);
                query.Append('&');
            }
        return TransformOutbound(query.Remove(query.Length - 1, 1).ToString());
    }
    internal static string TransformOutbound(string route) =>

        Regex.Replace(route,
                      "([a-z])([A-Z])",
                      "$1-$2",
                      RegexOptions.CultureInvariant,
                      TimeSpan.FromMilliseconds(0x64))

             .ToLowerInvariant();
}