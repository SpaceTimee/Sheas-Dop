using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using OnaCore;
using Sheas_Dop.Consts;
using Sheas_Dop.Utils;

namespace Sheas_Dop.Wins;

public partial class MainWin : Window
{
    private readonly HttpClient MainClient = new();
    private static readonly DispatcherTimer MihomoTimer = new() { Interval = TimeSpan.FromSeconds(0.1) };
    private static string? SingleUrl;
    private static bool IsMihomoRunning = false;

    public MainWin() => InitializeComponent();
    protected override void OnSourceInitialized(EventArgs e) => IconRemover.RemoveIcon(this);
    private async void MainWin_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Run(() =>
        {
            MihomoTimer.Tick += MihomoTimer_Tick;
            MihomoTimer.Start();
        });
    }
    private void MainWin_Closing(object sender, CancelEventArgs e) => Application.Current.Shutdown();

    private void SingleUrlBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (SingleButton.IsEnabled = MainConst.SingleUrlRegex().IsMatch(SingleUrlBox.Text))
            SingleUrl = SingleUrlBox.Text;
    }

    private async void GlobalButton_Click(object sender, RoutedEventArgs e)
    {
        if (!IsMihomoRunning)
        {
            if (MessageBox.Show("使用完请务必记得回来手动关闭代理，是否继续?", string.Empty, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            await Task.Run(() =>
            {
                new MihomoProc().ShellRun(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "-d .");
            });
        }
        else
            foreach (Process mihomoProcess in Process.GetProcessesByName("Dopping-Mihomo"))
            {
                mihomoProcess.Kill();
                await mihomoProcess.WaitForExitAsync();
            }
    }
    private async void SingleButton_Click(object sender, RoutedEventArgs e)
    {
        string singleDomain = SingleUrl!.TrimStart("http://".ToCharArray()).TrimStart("https://".ToCharArray());

        int portStartIndex = singleDomain.IndexOf(':');
        int pathStartIndex = singleDomain.IndexOf('/');

        if (portStartIndex != -1)
            singleDomain = singleDomain.Remove(portStartIndex);
        if (pathStartIndex != -1)
            singleDomain = singleDomain.Remove(pathStartIndex);

        if (JsonDocument.Parse(await Http.GetAsync<string>($"https://ns.net.kg/dns-query?name={singleDomain}", MainClient)).RootElement.TryGetProperty("Answer", out JsonElement arashiAnswers))
        {
            foreach (JsonElement ip in arashiAnswers.EnumerateArray().Distinct().ToList())
                if (MessageBox.Show(ip.GetProperty("data").ToString(), string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    break;
        }
        else
            MessageBox.Show("解不出来，我很抱歉");
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e) => MessageBox.Show($"欢迎使用 Sheas Dop {Assembly.GetExecutingAssembly().GetName().Version!.ToString()[0..^2]} (Sheas Cealer 子项目)，开发者 Space Time，反馈群 338919498");

    private void MihomoTimer_Tick(object? sender, EventArgs e)
    {
        if (IsMihomoRunning = Process.GetProcessesByName("Dopping-Mihomo").Length != 0)
        {
            GlobalButton.Content = "停止净化";
            GlobalButton.ToolTip = "点击停止全局净化";
        }
        else
        {
            GlobalButton.Content = "全局净化";
            GlobalButton.ToolTip = "点击启动全局净化";
        }
    }
    private void MainWin_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W)
            Application.Current.Shutdown();
    }
}