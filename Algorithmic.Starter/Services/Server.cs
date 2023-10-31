using ShareInvest.Properties;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ShareInvest.Services;

static class Server
{
    internal static bool Activate()
    {
        var files = new Stack<Models.File>();
        var parent = Directory.GetParent(Environment.CurrentDirectory);

        if (parent == null)
        {
            return false;
        }
        foreach (var file in Directory.GetFiles(parent.FullName, Resources.EXE, SearchOption.AllDirectories))
        {
            if (!nameof(Resources.ANT).Equals(Path.GetFileNameWithoutExtension(file), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var info = FileVersionInfo.GetVersionInfo(file);

            files.Push(new Models.File(new FileInfo(file).DirectoryName, info.FileVersion, info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart));
        }
        if (files.Count == 2)
        {
            int majorPart = int.MinValue, minorPart = int.MinValue, buildPart = int.MinValue, privatePart = int.MinValue;

            var directoryName = string.Empty;

            while (files.TryPop(out Models.File file))
            {
                if (string.IsNullOrEmpty(file.DirectoryName))
                {
                    continue;
                }
                if (majorPart <= file.MajorPart)
                {
                    if (minorPart <= file.MinorPart)
                    {
                        if (buildPart <= file.BuildPart)
                        {
                            if (privatePart <= file.PrivatePart)
                            {
                                if (privatePart < file.PrivatePart)
                                {
                                    directoryName = file.DirectoryName;
                                }
                                privatePart = file.PrivatePart;
                            }
                            else
                            {
                                continue;
                            }
                            buildPart = file.BuildPart;
                        }
                        else
                        {
                            continue;
                        }
                        minorPart = file.MinorPart;
                    }
                    else
                    {
                        continue;
                    }
                    majorPart = file.MajorPart;
                }
                else
                {
                    continue;
                }
            }
            if (string.IsNullOrEmpty(directoryName) is false && parent.FullName.Equals(directoryName) is false)
            {
                var removePath = directoryName.Replace(parent.FullName, string.Empty);

                foreach (var file in Directory.GetFiles(directoryName, "*", SearchOption.AllDirectories))
                {
                    var destFileName = file.Replace(removePath, string.Empty);

                    if (Path.GetDirectoryName(destFileName) is string directory)
                    {
                        var di = new DirectoryInfo(directory);

                        if (di.Exists is false)
                        {
                            di.Create();
                        }
                    }
                    File.Copy(file, destFileName, true);
                }
            }
            return privatePart > 0;
        }
        return false;
    }
    internal static bool Update()
    {
        foreach (var process in Process.GetProcessesByName(nameof(Resources.ANT)))
        {
            process.Kill(IsActived);
        }
        return Activate();
    }
    internal static void StartProcess()
    {
        var workingDirectory = Directory.GetParent(Environment.CurrentDirectory);

        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = string.Concat(nameof(Resources.ANT), Resources.EXE[1..]),
                WorkingDirectory = workingDirectory?.FullName,
                Verb = Resources.ADMIN
            }
        })
            if (process.Start())
            {
                GC.Collect();
            }
    }
    internal static bool IsActived
    {
        get => Process.GetProcessesByName(nameof(Resources.ANT)).Length == 1;
    }
}