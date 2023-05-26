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
        menu.Items.AddRange(new[]
        {
            new System.Windows.Forms.ToolStripMenuItem
            {
                Name = nameof(Properties.Resources.REGISTER),
                Text = IsRegistered ? Properties.Resources.UNREGISTER : Properties.Resources.REGISTER
            },
            new System.Windows.Forms.ToolStripMenuItem
            {
                Name = nameof(Properties.Resources.UPDATE),
                Text = Properties.Resources.UPDATE
            },
            new System.Windows.Forms.ToolStripMenuItem
            {
                Name = nameof(Properties.Resources.EXIT),
                Text = Properties.Resources.EXIT
            }
        });
        icons = new[]
        {
             Properties.Resources.bird_sleep,
             Properties.Resources.bird_idle,
             Properties.Resources.bird_awake,
             Properties.Resources.bird_alert,
             Properties.Resources.bird_away,
             Properties.Resources.bird_invisible
        };
        notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            ContextMenuStrip = menu,
            Visible = true,
            Text = Properties.Resources.TITLE,
            Icon = icons[^1],
            BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        };
        timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 1)
        };
        menu.ItemClicked += (sender, e) =>
        {
            if (nameof(Properties.Resources.UPDATE).Equals(e.ClickedItem?.Name))
            {
                if (server.Update())
                {
                    server.StartProcess();
                }
                else
                {
                    notifyIcon.Text = Properties.Resources.ERROR;
                }
                return;
            }
            if (nameof(Properties.Resources.REGISTER).Equals(e.ClickedItem?.Name))
            {
                e.ClickedItem.Text =

                    Properties.Resources.UNREGISTER.Equals(e.ClickedItem.Text) ? Properties.Resources.REGISTER : Properties.Resources.UNREGISTER;

                var fileName = string.Concat(Assembly.GetEntryAssembly()?.ManifestModule.Name[..^4], Properties.Resources.EXE[1..]);

                register.IsWritable = register.IsWritable is false;

                var res = register.AddStartupProgram(Properties.Resources.TITLE, fileName);

                if (string.IsNullOrEmpty(res) is false && notifyIcon != null)
                {
                    notifyIcon.Text = res;
                }
                return;
            }
            IsUserClosing = true;

            Close();
        };
        timer.Tick += (sender, e) =>
        {
            if (server.IsActived)
            {
                notifyIcon.Icon = icons[DateTime.Now.Second % 5];
            }
            else
            {
                timer.Interval = new TimeSpan(1, 1, 1, 0xC);

                if (server.Activate())
                {
                    server.StartProcess();

                    timer.Interval = new TimeSpan(0, 0, 1);
                }
                else
                {
                    notifyIcon.Text = Properties.Resources.ERROR;
                }
                notifyIcon.Icon = icons[^1];
            }
        };
        InitializeComponent();

        Title = nameof(Starter);

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

        GC.Collect();
    }
    bool IsUserClosing
    {
        get; set;
    }
    bool IsRegistered
    {
        get => register.GetValue(Properties.Resources.TITLE);
    }
    readonly System.Windows.Forms.ContextMenuStrip menu;
    readonly System.Windows.Forms.NotifyIcon notifyIcon;
    readonly System.Drawing.Icon[] icons;
    readonly DispatcherTimer timer;
    readonly Server server = new();
    readonly Register register = new(Properties.Resources.RUN);
}