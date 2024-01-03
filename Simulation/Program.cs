using Microsoft.Extensions.Configuration;

using RestSharp;

var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                              .AddJsonFile("appsettings.json")
                                              .Build();
var cts = new CancellationTokenSource();

string url =
#if DEBUG
    configuration.GetConnectionString("Url")!;
#else
    "http://share.enterprises";
#endif
using (var client = new RestClient(url, configureDefaultHeaders: headers =>
{
    headers.Add(KnownHeaders.Authorization, $"Bearer {args[0]}");
}))
{

}