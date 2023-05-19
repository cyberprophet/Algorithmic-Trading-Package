using Microsoft.Extensions.Configuration;

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
                                      .AddJsonFile(settings)
                                      .Build();
    }
    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        foreach (var arg in e.Args)
        {
            Debug.WriteLine(arg);
        }
        base.OnStartup(e);
#else
        const string admin = "Administrator";
        const string execute = "Execute";

        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Verb = Configuration.GetConnectionString(admin),
                UseShellExecute = true,
                FileName = string.Concat(Assembly.GetEntryAssembly()?.ManifestModule.Name[..^4],
                                         Configuration.GetConnectionString(execute))
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
    const string settings = "appsettings.json";
}