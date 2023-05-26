using System.Runtime.Versioning;
using System.Security.Principal;

namespace ShareInvest.Services;

static class Status
{
    [SupportedOSPlatform("windows10.0")]
    internal static bool IsAdministrator
    {
        get
        {
            using (var cur = WindowsIdentity.GetCurrent())
            {
                return new WindowsPrincipal(cur).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}