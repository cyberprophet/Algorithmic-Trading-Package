using Microsoft.Extensions.Configuration;

using ShareInvest.Services;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ShareInvest;

public partial class Install : Window
{
    public Install()
    {
        menu = new System.Windows.Forms.ContextMenuStrip
        {
            Cursor = System.Windows.Forms.Cursors.Hand
        };
        menu.Items.AddRange(new[]
        {
            new System.Windows.Forms.ToolStripMenuItem
            {
                Name = nameof(Properties.Resources.EXIT),
                Text = Properties.Resources.EXIT
            }
        });
        menu.ItemClicked += (sender, e) =>
        {
            Visibility = Visibility.Hidden;

            Close();
        };
        icons = new[]
        {
             Properties.Resources.bird_invisible,
             Properties.Resources.bird_sleep,
             Properties.Resources.bird_idle,
             Properties.Resources.bird_awake,
             Properties.Resources.bird_alert,
             Properties.Resources.bird_away,
        };
        notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            ContextMenuStrip = menu,
            Visible = true,
            Text = Properties.Resources.TITLE,
            Icon = icons[^1],
            BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        };
        notifyIcon.MouseDoubleClick += (sender, e) =>
        {
            if (IsVisible == false)
            {
                Show();
            }
        };
        timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 1)
        };
        timer.Tick += async (sender, e) =>
        {
            notifyIcon.Icon = icons[DateTime.Now.Second % 6];

            await InitializeCoreWebView2Async();
        };
        InitializeComponent();

        var hRgn = WindowAttribute.CreateRoundRectRgn(0, 0, menu.Width, menu.Height, 9, 9);
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

        WindowAttribute.DwmSetWindowAttribute(new WindowInteropHelper(this).EnsureHandle(),
                                              DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                                              ref preference,
                                              sizeof(uint));

        _ = WindowAttribute.SetWindowRgn(menu.Handle, hRgn, true);

        Title = Properties.Resources.TITLE;

        timer.Start();
    }
    async Task InitializeCoreWebView2Async()
    {
        if (launcher.Source != null)
        {
            return;
        }
        await launcher.EnsureCoreWebView2Async();

        launcher.Source =

            new Uri(App.Configuration.GetConnectionString(nameof(Properties.Resources.Url)) ??

                    Properties.Resources.Url);
    }
    void OnStateChanged(object sender, EventArgs e)
    {
        if (WindowState.Normal != WindowState)
        {
            Hide();
        }
    }
    void OnClosing(object sender, CancelEventArgs e)
    {
        if (MessageBoxResult.Cancel.Equals(MessageBox.Show(Properties.Resources.WARNING.Replace('|', '\n'),
                                                           Title,
                                                           MessageBoxButton.OKCancel,
                                                           MessageBoxImage.Warning,
                                                           MessageBoxResult.Cancel)))
        {
            e.Cancel = true;

            return;
        }
        GC.Collect();
    }
    readonly System.Windows.Forms.ContextMenuStrip menu;
    readonly System.Windows.Forms.NotifyIcon notifyIcon;
    readonly System.Drawing.Icon[] icons;
    readonly DispatcherTimer timer;
}