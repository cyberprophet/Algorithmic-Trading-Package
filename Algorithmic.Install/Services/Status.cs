using System;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace ShareInvest.Services;

static class Status
{
    [SupportedOSPlatform("windows8.0")]
    internal static bool IsAdministrator
    {
        get
        {
            using (var windows = WindowsIdentity.GetCurrent())
            {
                return new WindowsPrincipal(windows).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
    internal static string TransformOutbound(string route) =>

        Regex.Replace(route,
                      "([a-z])([A-Z])",
                      "$1-$2",
                      RegexOptions.CultureInvariant,
                      TimeSpan.FromMilliseconds(0x64))
             
             .ToLowerInvariant();
}