using ShareInvest.Properties;

using System;
using System.Diagnostics;
using System.IO;

namespace ShareInvest.Services;

static class App
{
    internal static void Activate()
    {
        var dirInfo = new DirectoryInfo(Resources.PATH);
        var parent = Directory.GetParent(Environment.CurrentDirectory);
        var latestPath = string.Empty;

        if (parent == null)
        {
            return;
        }
        foreach (var file in Directory.GetFiles(parent.FullName, Resources.EXE, SearchOption.AllDirectories))
        {
            if (!Resources.SECURITIES.Equals(Path.GetFileName(file), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            latestPath = Path.GetDirectoryName(file) ?? string.Empty;
        }
        if (dirInfo.Exists is false)
        {
            dirInfo.Create();
        }
        foreach (var file in Directory.GetFiles(latestPath, "*", SearchOption.AllDirectories))
        {
            var latestFileInfo = new FileInfo(file);

            if (string.IsNullOrEmpty(latestFileInfo.DirectoryName))
            {
                continue;
            }
            var presentFileInfo = new FileInfo(file.Replace(latestPath, dirInfo.FullName));

            if (presentFileInfo.Exists)
            {
                var latest = FileVersionInfo.GetVersionInfo(file);
                var present = FileVersionInfo.GetVersionInfo(presentFileInfo.FullName);

                if (string.IsNullOrEmpty(latest.FileVersion) is false && latest.FileVersion.Equals(present.FileVersion))
                {
                    continue;
                }
            }
            else if (Path.GetDirectoryName(presentFileInfo.FullName) is string dirName)
            {
                var presentDir = new DirectoryInfo(dirName);

                if (presentDir.Exists is false)
                {
                    presentDir.Create();
                }
            }
            File.Copy(file, presentFileInfo.FullName, true);
        }
    }
    internal static void StartProcess()
    {
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Resources.SECURITIES,
                WorkingDirectory = Resources.PATH,
                Verb = Resources.ADMIN
            }
        })
        {
            if (process.Start())
            {
                GC.Collect();
            }
        }
    }
    internal static bool IsActived
    {
        get => Process.GetProcessesByName(Resources.SECURITIES[..^4]).Length > 0;
    }
}