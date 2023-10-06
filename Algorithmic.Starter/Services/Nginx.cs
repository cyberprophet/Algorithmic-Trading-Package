using ShareInvest.Properties;

using System;
using System.Diagnostics;
using System.IO;

namespace ShareInvest.Services;

static class Nginx
{
    internal static bool BeOutOperation
    {
        get => Process.GetProcessesByName(nameof(Nginx)).Length == 0;
    }
    internal static void StartProcess()
    {
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = string.Concat(nameof(Nginx), Resources.EXE[1..]),
                WorkingDirectory = Resources.NGINX
            }
        })
        {
            if (process.Start())
            {
                foreach (var command in new[] { Resources.ACCESS, Resources.ERROR })
                {
                    using (var p = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = Resources.POWERSHELL,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            WorkingDirectory = Path.Combine(Resources.NGINX, Resources.PATH)
                        }
                    })
                    {
                        if (p.Start())
                        {
                            p.StandardInput.WriteLine(command + Environment.NewLine);
                            p.StandardInput.Close();
                        }
                    }
                }
            }
        }
        GC.Collect();
    }
}