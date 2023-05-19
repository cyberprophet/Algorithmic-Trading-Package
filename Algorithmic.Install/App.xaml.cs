using Microsoft.Extensions.Configuration;

using ShareInvest.Services;

using System;
using System.Diagnostics;
using System.Reflection;
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
        if (Status.IsAdministrator)
        {
            base.OnStartup(e);

            return;
        }
#if DEBUG
        foreach (var arg in e.Args)
        {
            Debug.WriteLine(arg);
        }
#else
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Verb = Configuration.GetConnectionString(ShareInvest.Properties.Resources.ADMIN),
                UseShellExecute = true,
                FileName = string.Concat(Assembly.GetEntryAssembly()?.ManifestModule.Name[..^4],
                                         Configuration.GetConnectionString(ShareInvest.Properties.Resources.EXECUTE))
            }
        })
            if (process.Start())
            {
                GC.Collect();
            }
            else
                MessageBox.Show(process.StartInfo.WorkingDirectory,
                                process.ProcessName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

        Process.GetCurrentProcess().Kill();
#endif
    }
}