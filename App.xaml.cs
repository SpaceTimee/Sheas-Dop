using System.Windows;

namespace Sheas_Dop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e) => new MainWindow().Show();
}