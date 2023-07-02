using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace N2NGUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private bool IsUserAdmin()
    {
        WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(currentIdentity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private void Load()
    {
        var data = Config.LoadConfig();
        ServerAddressBox.Text = data.Server;
        LanAddressBox.Text = data.LanAddress;
        CoummityBox.Text = data.Community;
        // EncryptTypeComboBox
        EncryptPasswordBox.Text = data.Password;
    }

    private void Save()
    {
        Config.SaveConfig(
            server: ServerAddressBox.Text,
            lanAddress: LanAddressBox.Text,
            community: CoummityBox.Text,
            type: Data.Types.EnyptType.Aes,
            password: EncryptPasswordBox.Text
        );
    }

    public MainWindow()
    {
        if (IsUserAdmin())
        {
            InitializeComponent();
            Load();
        }
        else
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Process.GetCurrentProcess().MainModule?.FileName;
            startInfo.Verb = "runas";
            try
            {
                Process.Start(startInfo);
            }
            finally
            {
                Environment.Exit(0);
                //退出
            }
        }
    }

    private void LaunchGitHubSite(object sender, RoutedEventArgs e)
    {
        Process.Start("https://github.com/Jkkoi/N2NGUI");
    }


    private void ControlBtn(object sender, RoutedEventArgs e)
    {
        Save();
        N2NHelper n2nHelper = N2NHelper.Instance;
        if (ControlBtnTextBlock.Text == "开启")
        {
            PrintLog(this, Config.GenerateN2NArguments());
            n2nHelper.LogEvent += PrintLog;
            n2nHelper.RunN2N();

            ControlBtnTextBlock.Text = "关闭";
        }
        else
        {
            n2nHelper.LogEvent -= PrintLog;
            n2nHelper.ShutDownN2N();
            ControlBtnTextBlock.Text = "开启";
        }
    }

    private void PrintLog(object? sender, string e)
    {
        LogBlock.Text += e + Environment.NewLine;
        LogBlock.ScrollToEnd();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        N2NHelper.Instance.ShutDownN2N();
        e.Cancel = false;
    }
}