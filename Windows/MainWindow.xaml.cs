using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using OnaCore;

namespace Sheas_Dop;

public partial class MainWindow : Window
{
    private static readonly DispatcherTimer MONITOR_TIMER = new() { Interval = new TimeSpan(1000000) };  //0.1s
    private readonly HttpClient MAIN_CLIENT = new();    //当前窗口使用的唯一的 HttpClient

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
            List<string> ips = [];

            JToken arashiAnswers = await DomainResolve($"https://ns.net.kg/dns-query?name={DomainBox.Text}&type=A");
            if (arashiAnswers != null)
                foreach (JToken arashiAnswer in arashiAnswers)
                    ips.Add(arashiAnswer["data"]!.ToString());

            JToken quad101Answers = await DomainResolve($"https://101.101.101.101/dns-query?name={DomainBox.Text}&type=A");
            if (quad101Answers != null)
                foreach (JToken quad101Answer in quad101Answers)
                    ips.Add(quad101Answer["data"]!.ToString());

            if (ips.Count > 0)
            {
                ips = ips.Distinct().ToList();
                foreach (string ip in ips)
                    if (MessageBox.Show(ip, "解析结果", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) break;
            }
            else
                MessageBox.Show("没有符合的解析结果", "解析结果");
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }
    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        try { MessageBox.Show("欢迎使用 Sheas Dop " + Assembly.GetExecutingAssembly().GetName().Version!.ToString()[0..^2] + "，开发者 Space Time，反馈群 338919498"); }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }

    private async Task<JToken> DomainResolve(string resolverUrl) => JObject.Parse(await Http.GetAsync<string>(resolverUrl, MAIN_CLIENT))["Answer"]!;

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