﻿using ShareInvest.Services;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace ShareInvest;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        if (Status.IsAdministrator)
        {
            base.OnStartup(e);

            return;
        }
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Verb = ShareInvest.Properties.Resources.ADMIN,
                UseShellExecute = true,
                FileName = string.Concat(Assembly.GetEntryAssembly()?.ManifestModule.Name[..^4],
                                         ShareInvest.Properties.Resources.EXE[1..])
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
    }
}