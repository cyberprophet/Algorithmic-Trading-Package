using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ShareInvest.Services;

static class Install
{
    internal static IEnumerable<FileVersionInfo> GetVersionInfo(string fileName)
    {
        string? dirName = string.Empty;
        string path = App.Configuration.GetConnectionString(Properties.Resources.SOURCES) ?? Properties.Resources.PATH;

        foreach (var file in Directory.EnumerateFiles(path, fileName, SearchOption.AllDirectories))
        {
            var info = FileVersionInfo.GetVersionInfo(file);

            dirName = Path.GetDirectoryName(info.FileName);

            if (string.IsNullOrEmpty(dirName) is false &&
                dirName.EndsWith(Properties.Resources.PUBLISH, StringComparison.OrdinalIgnoreCase))
            {
#if DEBUG
                Debug.WriteLine(JsonConvert.SerializeObject(info, Formatting.Indented));

                Build(dirName, Properties.Resources.BUILD);
#endif
                break;
            }
        }
        foreach (var file in Directory.EnumerateFiles(dirName, "*", SearchOption.AllDirectories))
        {
#if DEBUG
            Debug.WriteLine(file);
#endif
            yield return FileVersionInfo.GetVersionInfo(file);
        }
    }
    static void Build(string workingDirectory, string commandLine)
    {
        DirectoryInfo? directoryInfo;

        while (workingDirectory.Length > 0)
        {
            directoryInfo = Directory.GetParent(workingDirectory);

            if (directoryInfo != null)
            {
                workingDirectory = directoryInfo.FullName;

                if (directoryInfo?.GetFiles(Properties.Resources.CSPROJ, SearchOption.TopDirectoryOnly).Length > 0)
                {
                    break;
                }
            }
        }
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                Verb = App.Configuration.GetConnectionString(Properties.Resources.ADMIN),
                FileName = Properties.Resources.POWERSHELL,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        })
        {
#if DEBUG
            process.OutputDataReceived += (sender, e) =>
            {
                Debug.WriteLine(e.Data);
            };
#endif
            if (process.Start())
            {
                process.BeginOutputReadLine();
                process.StandardInput.Write(commandLine + Environment.NewLine);
                process.StandardInput.Close();
                process.WaitForExit();
            }
        }
    }
}