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
        string access = nameof(Resources.ACCESS), error = nameof(Resources.ERROR), log = nameof(Resources.LOG);

        static void renameFile(string filePath)
        {
            if (File.Exists(filePath) && Path.GetDirectoryName(filePath) is string directory)
            {
                var newFileName = string.Concat(Path.GetFileNameWithoutExtension(filePath), '-', DateTime.Now.ToString("d"), '.', nameof(log));

                File.Move(filePath, Path.Combine(directory, newFileName), false);
            }
        }
        var workingDirectory = Path.Combine(Resources.NGINX, Resources.LOG);

        renameFile(string.Concat(workingDirectory, '\\', nameof(access), '.', nameof(log)));
        renameFile(string.Concat(workingDirectory, '\\', nameof(error), '.', nameof(log)));

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
                            WorkingDirectory = workingDirectory
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