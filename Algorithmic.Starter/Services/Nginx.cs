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
        if (Directory.GetParent(Resources.PATH) is DirectoryInfo workingDirectory)
        {
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = string.Concat(nameof(Nginx), Resources.EXE[1..]),
                    WorkingDirectory = workingDirectory.FullName
                }
            })
                if (process.Start())
                {
                    using (var p = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = Resources.POWERSHELL,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            WorkingDirectory = Resources.PATH
                        }
                    })
                        if (p.Start())
                        {
                            p.StandardInput.WriteLine(Resources.LOG + Environment.NewLine);
                            p.StandardInput.Close();
                        }
                    GC.Collect();
                }
        }
    }
}