using Microsoft.Extensions.Configuration;

using System;
using System.Windows;

namespace ShareInvest;

public partial class App : Application
{
    public static IConfigurationRoot Configuration
    {
        get =>

            new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                      .AddJsonFile(ShareInvest.Properties.Resources.SETTINGS)
                                      .Build();
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
    }
}