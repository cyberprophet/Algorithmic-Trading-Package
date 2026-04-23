using ShareInvest.Services;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace ShareInvest;

public partial class Starter : Window
{
    public Starter()
    {
        register.IsWritable = IsRegistered;

        menu = new System.Windows.Forms.ContextMenuStrip
        {
            Cursor = System.Windows.Forms.Cursors.Hand
        };
        menu.Items.AddRange(
        [
            new System.Windows.Forms.ToolStripMenuItem
            {
                Name = nameof(Properties.Resources.REGISTER),
                Text = IsRegistered ? Properties.Resources.UNREGISTER : Properties.Resources.REGISTER
            },
            new System.Windows.Forms.ToolStripMenuItem
            {
                Name = nameof(Properties.Resources.EXIT),
                Text = Properties.Resources.EXIT
            }
        ]);
        icons =
        [
            Properties.Resources.UPLOAD,
            Properties.Resources.DOWNLOAD,
            Properties.Resources.IDLE
        ];
        notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            ContextMenuStrip = menu,
            Visible = true,
            Text = Properties.Resources.ANT,
            Icon = Properties.Resources.BLACK,
            BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        };
        timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 1)
        };
        menu.ItemClicked += (_, e) =>
        {
            if (nameof(Properties.Resources.REGISTER).Equals(e.ClickedItem?.Name))
            {
                e.ClickedItem.Text = Properties.Resources.UNREGISTER.Equals(e.ClickedItem.Text) ? Properties.Resources.REGISTER : Properties.Resources.UNREGISTER;

                var fileName = string.Concat(Assembly.GetEntryAssembly()?.ManifestModule.Name[..^4], Properties.Resources.EXE[1..]);

                register.IsWritable = register.IsWritable is false;

                var res = register.AddStartupProgram(Properties.Resources.ANT, fileName);

                if (string.IsNullOrEmpty(res) is false && notifyIcon != null)
                {
                    notifyIcon.Text = res;
                }
                return;
            }
            IsUserClosing = true;

            Close();
        };
        timer.Tick += (sender, _) =>
        {
            if (Nginx.BeOutOperation)
            {
                timer.Interval = new TimeSpan(1, 1, 1, 0xC);

                notifyIcon.Icon = icons[^1];
                notifyIcon.Text = $"{DateTime.Now:g}";

                Nginx.StartProcess();

                timer.Interval = new TimeSpan(0, 0, 1);
            }
            else if (AnTalk.BeOutOperation)
            {
                timer.Interval = new TimeSpan(1, 1, 1, 0xC);

                AnTalk.StartProcess();

                timer.Interval = new TimeSpan(0, 0, 1);
            }
            else
            {
                notifyIcon.Icon = icons[DateTime.Now.Second % 2];
            }
        };
        InitializeComponent();

        Title = nameof(Nginx);

        timer.Start();

        Visibility = Visibility.Hidden;
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
        if (IsUserClosing && MessageBoxResult.Cancel.Equals(MessageBox.Show(Properties.Resources.WARNING.Replace('|', '\n'), Title, MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel)))
        {
            e.Cancel = true;

            return;
        }
        IsUserClosing = false;

        GC.Collect();
    }

    bool IsUserClosing
    {
        get; set;
    }

    bool IsRegistered
    {
        get => register.GetValue(Properties.Resources.ANT);
    }

    readonly System.Windows.Forms.ContextMenuStrip menu;
    readonly System.Windows.Forms.NotifyIcon notifyIcon;
    readonly System.Drawing.Icon[] icons;

    readonly DispatcherTimer timer;

    readonly Register register = new(Properties.Resources.RUN);
}