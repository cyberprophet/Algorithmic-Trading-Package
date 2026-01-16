using ShareInvest.Services;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
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
                Name = nameof(Properties.Resources.THEME),
                Text = SearchingTheme ? Properties.Resources.SEARCHINGTHEME : Properties.Resources.THEME
            },
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

        menu.ItemClicked += (sender, e) =>
        {
            if (nameof(Properties.Resources.THEME).Equals(e.ClickedItem?.Name))
            {
                SearchingTheme = SearchingTheme is false;

                _ = Task.Run(async () =>
                {
                    while (SearchingTheme) await Task.Delay(0x400 * Update.InquiryProcess());
                });
                e.ClickedItem.Text = SearchingTheme ? Properties.Resources.SEARCHINGTHEME : Properties.Resources.THEME;

                return;
            }

            if (nameof(Properties.Resources.UPDATE).Equals(e.ClickedItem?.Name))
            {
                if (Server.Update())
                {
                    notifyIcon.Text = $"{DateTime.Now:g}\n[{Properties.Resources.UPDATE}] {Properties.Resources.NOTICE}";
                }
                return;
            }

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
        }
            ;

        timer.Tick += async (sender, e) =>
        {
            // WINDOWS SERVER
            // if (Server.IsActived)
            {
                if (Services.App.IsActived is false)
                {
                    timer.Interval = new TimeSpan(1, 1, 1, 0xC);

                    await Task.Delay(Random.Shared.Next(3 * 0x400, 0xA * 0x400));

                    Services.App.Activate();
                    Services.App.StartProcess();

                    timer.Interval = new TimeSpan(0, 0, 1);
                }
                notifyIcon.Icon = icons[DateTime.Now.Second % 2];
            }

            /* WINDOWS SERVER
            else
            {
                timer.Interval = new TimeSpan(1, 1, 1, 0xC);

                if (Server.Activate())
                {
                    notifyIcon.Icon = icons[^1];
                }
                else
                {
                    notifyIcon.Text = $"{DateTime.Now:g}\n[{nameof(Server.StartProcess)}] {Properties.Resources.NOTICE}";
                }
                Server.StartProcess(DateTime.Now.ToString("d"));

                timer.Interval = new TimeSpan(0, 0, 1);
            }
            */
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

    bool SearchingTheme
    {
        get; set;
    }

    readonly System.Windows.Forms.ContextMenuStrip menu;
    readonly System.Windows.Forms.NotifyIcon notifyIcon;
    readonly System.Drawing.Icon[] icons;

    readonly DispatcherTimer timer;

    readonly Register register = new(Properties.Resources.RUN);
}