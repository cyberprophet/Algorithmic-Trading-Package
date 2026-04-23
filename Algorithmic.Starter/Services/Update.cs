using ShareInvest.Properties;

using System;
using System.Diagnostics;

namespace ShareInvest.Services;

static class Update
{
    internal static int InquiryProcess()
    {
        if (Process.GetProcessesByName(Resources.INQUIRY).Length > 0)
        {
            return Random.Shared.Next(0xA, 0xF);
        }
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = string.Concat(Resources.INQUIRY, ".exe"),
                WorkingDirectory = "C:\\AnTalk\\INQUIRY",
                Verb = "runas"
            }
        })
        {
            if (process.Start())
            {
                GC.Collect();
            }
        }
        return Random.Shared.Next(1, 0x10);
    }
}