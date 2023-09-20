using ShareInvest.Properties;

using System;
using System.Diagnostics;
using System.IO;

namespace ShareInvest.Services;

static class Nginx
{
    internal static bool BeOutOperation
    {
        get => Process.GetProcessesByName(string.Concat(nameof(Nginx), Resources.EXE[1..])).Length == 0;
    }
    internal static void StartProcess()
    {
        if (Directory.GetParent(Environment.CurrentDirectory) is DirectoryInfo workingDirectory)
        {
            var fullName = Directory.GetParent(workingDirectory.FullName)?.FullName ?? string.Empty;

            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = string.Concat(nameof(Nginx), Resources.EXE[1..]),
                    WorkingDirectory = Path.Combine(fullName, Resources.NGINX)
                }
            })
            {
                if (process.Start())
                {

                }
                GC.Collect();
            }
        }
    }
}