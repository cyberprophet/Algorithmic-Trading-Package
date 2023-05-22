using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using RestSharp;

using ShareInvest.Services;

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
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
        menu.ItemClicked += async (sender, e) =>
        {
            switch (e.ClickedItem?.Name)
            {
                case nameof(Properties.Resources.LAUNCHER):

                    await ExecuteAsync(Properties.Resources.LAUNCHER);

                    return;
            }
            IsUserClosing = true;

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
            var index = DateTime.Now.Second % 6;

            if (index == 0)
            {
                await InitializeCoreWebView2Async();
            }
            notifyIcon.Icon = icons[index];
        };
        InitializeComponent();

        Title = Properties.Resources.TITLE;

        var hRgn = WindowAttribute.CreateRoundRectRgn(0, 0, menu.Width, menu.Height, 9, 9);
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

        WindowAttribute.DwmSetWindowAttribute(new WindowInteropHelper(this).EnsureHandle(),
                                              DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                                              ref preference,
                                              sizeof(uint));

        _ = WindowAttribute.SetWindowRgn(menu.Handle, hRgn, true);

        timer.Start();
    }
    async Task InitializeCoreWebView2Async()
    {
        if (launcher.Source != null)
        {
            var path = App.Configuration.GetConnectionString(nameof(Properties.Resources.PATH));

            if (string.IsNullOrEmpty(path) is false)
            {
                DirectoryInfo di = new(path);

                if (di.Exists)
                {
                    timer.Stop();

                    if (await Status.StartProcess(Properties.Resources.LAUNCHER))
                    {
                        Close();
                    }
                }
                else
                {
                    di.Create();
                }
            }
            return;
        }
        await launcher.EnsureCoreWebView2Async();

        launcher.Source =

            new Uri(App.Configuration.GetConnectionString(nameof(Uri)) ?? Properties.Resources.PATH);
    }
    async Task ExecuteAsync(string program)
    {
        var fileName = string.Concat(program, App.Configuration.GetConnectionString(Properties.Resources.EXECUTE));

        foreach (var info in Services.Install.GetVersionInfo(fileName))
        {
            var index = info.FileName.IndexOf(Properties.Resources.PUBLISH, StringComparison.OrdinalIgnoreCase);

            if (index < 0)
                continue;

            var route = string.Concat(App.Configuration.GetConnectionString(Properties.Resources.ROUTE),
                                      '/',
                                      Status.TransformOutbound(info.GetType().Name));

            var request = new RestRequest(route, Method.POST);

            request.AddJsonBody(JsonConvert.SerializeObject(new Models.FileVersionInfo
            {
                App = program,
                Path = Path.GetDirectoryName(info.FileName)?[index..],
                FileName = Path.GetFileName(info.FileName),
                CompanyName = info.CompanyName,
                FileBuildPart = info.FileBuildPart,
                FileDescription = info.FileDescription,
                FileMajorPart = info.FileMajorPart,
                FileMinorPart = info.FileMinorPart,
                FilePrivatePart = info.FilePrivatePart,
                FileVersion = info.FileVersion,
                InternalName = info.InternalName,
                OriginalFileName = info.OriginalFilename,
                PrivateBuild = info.PrivateBuild,
                ProductBuildPart = info.ProductBuildPart,
                ProductMajorPart = info.ProductMajorPart,
                ProductMinorPart = info.ProductMinorPart,
                ProductName = info.ProductName,
                ProductPrivatePart = info.ProductPrivatePart,
                ProductVersion = info.ProductVersion,
                Publish = DateTime.Now.Ticks,
                File = await System.IO.File.ReadAllBytesAsync(info.FileName)
            }));
            await client.ExecuteAsync(request, cancellationTokenSource.Token);
        }
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
        if (IsUserClosing &&
            MessageBoxResult.Cancel.Equals(MessageBox.Show(Properties.Resources.WARNING.Replace('|', '\n'),
                                                           Title,
                                                           MessageBoxButton.OKCancel,
                                                           MessageBoxImage.Warning,
                                                           MessageBoxResult.Cancel)))
        {
            e.Cancel = true;

            return;
        }
        IsUserClosing = false;
    }
    bool IsUserClosing
    {
        get; set;
    }
    readonly RestClient client = new()
    {
        BaseUrl = new Uri(App.Configuration.GetConnectionString(Properties.Resources.LAUNCHER) ?? Properties.Resources.PATH),
        Timeout = -1
    };
    readonly System.Windows.Forms.ContextMenuStrip menu;
    readonly System.Windows.Forms.NotifyIcon notifyIcon;
    readonly System.Drawing.Icon[] icons;
    readonly DispatcherTimer timer;
    readonly CancellationTokenSource cancellationTokenSource = new();
}