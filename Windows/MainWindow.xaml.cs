using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TurnerSoftware.DinoDNS;
using TurnerSoftware.DinoDNS.Protocol;
using TurnerSoftware.DinoDNS.Protocol.ResourceRecords;

namespace Sheas_Dop;

public partial class MainWindow : Window
{
    private static readonly DispatcherTimer MONITOR_TIMER = new() { Interval = new TimeSpan(1000000) };  //0.1s

    public MainWindow()
    {
        InitializeComponent();

        MONITOR_TIMER.Tick += MONITOR_TIMER_Tick;
        MONITOR_TIMER.Start();
    }

    private void MONITOR_TIMER_Tick(object? sender, EventArgs e)
    {
        try
        {
            if (Process.GetProcessesByName("Dopping-Clash").Length == 0)
                GlobalButton.Content = "全局解析";
            else
                GlobalButton.Content = "停止解析";
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }

    private void DomainBox_GotFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (DomainBox.Foreground != Foreground)
            {
                //PlaceHold 状态
                DomainBox.Text = string.Empty;
                DomainBox.Foreground = Foreground;
            }
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }
    private void DomainBox_LostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(DomainBox.Text))
            {
                DomainBox.Text = "(填入任意不带有 http(s):// 的域名)";
                DomainBox.Foreground = new SolidColorBrush(Color.FromRgb(191, 205, 219));
            }
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }
    private void DomainBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (SingleButton == null)
                return;

            SingleButton.IsEnabled = new Regex(@"^([a-zA-Z0-9\-]+\.)+[a-zA-Z0-9\-]+$").IsMatch(DomainBox.Text);
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }

    private void GlobalButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (GlobalButton.Content.ToString() == "全局解析")
                new Clash().ShellRun(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!, "-d .");
            else
                foreach (Process clashProcess in Process.GetProcessesByName("Dopping-Clash"))
                    clashProcess.Kill();
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }
    private async void SingleButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (GlobalButton.Content.ToString() != "全局解析")
                MessageBox.Show("使用单个解析时需停止全局解析");
            else
                await ResolveDNS(new DnsClient(new NameServer[] { new NameServer(IPAddress.Parse("1.1.1.1"), NameServers.DefaultDoTPort, ConnectionType.DoT) }, DnsMessageOptions.Default));

            //NameServers.Cloudflare.IPv4.GetPrimary(ConnectionType.DoH)
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }
    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        try { MessageBox.Show("欢迎使用 Sheas Dop " + Assembly.GetExecutingAssembly().GetName().Version!.ToString()[0..^2] + "，开发者 Space Time，反馈群 338919498"); }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }

    private async Task ResolveDNS(DnsClient client)
    {
        DnsMessage dnsMessage = await client.QueryAsync(DomainBox.Text, DnsQueryType.A);
        foreach (ARecord aRecord in dnsMessage.Answers.WithARecords())
            if (MessageBox.Show(aRecord.ToIPAddress().ToString(), "解析结果", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) break;
    }

    private void MainWin_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.W)
                Environment.Exit(0);
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }
}