using ShareInvest.Properties;

using System;
using System.Diagnostics;

namespace ShareInvest.Services;

class AnTalk
{
    internal static bool BeOutOperation
    {
        get => Process.GetProcessesByName(name).Length == 0;
    }

    internal static void StartProcess()
    {
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = string.Concat(name, Resources.EXE[1..]),
                WorkingDirectory = Resources.ANTALK
            }
        })
            if (process.Start()) GC.Collect();
    }

    readonly static string name = string.Concat(nameof(AnTalk), '.', nameof(Starter));
}