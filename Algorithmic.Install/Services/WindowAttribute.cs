using System;
using System.Runtime.InteropServices;

namespace ShareInvest.Services;

static partial class WindowAttribute
{
    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmSetWindowAttribute(IntPtr hwnd,
                                                      DWMWINDOWATTRIBUTE attribute,
                                                      ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                      uint cbAttribute);

    [LibraryImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    internal static partial IntPtr CreateRoundRectRgn(int nLeftRect,
                                                     int nTopRect,
                                                     int nRightRect,
                                                     int nBottomRect,
                                                     int nWidthEllipse,
                                                     int nHeightEllipse);

    [LibraryImport("user32.dll")]
    internal static partial int SetWindowRgn(IntPtr hWnd,
                                            IntPtr hRgn,
                                            [MarshalAs(UnmanagedType.Bool)] bool bRedraw);
}
enum DWMWINDOWATTRIBUTE
{
    DWMWA_WINDOW_CORNER_PREFERENCE = 33
}
enum DWM_WINDOW_CORNER_PREFERENCE
{
    DWMWCP_DEFAULT = 0,
    DWMWCP_DONOTROUND = 1,
    DWMWCP_ROUND = 2,
    DWMWCP_ROUNDSMALL = 3
}