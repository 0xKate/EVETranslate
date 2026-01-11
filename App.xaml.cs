using EVETranslate.Services;
using EVETranslate.ViewModels;
using EVETranslate.Views;
using System.Windows;

namespace EVETranslate
{
    public partial class App : Application
    {
        static public SettingsService Settings { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(Settings)
            };

            mainWindow.Show();
        }
    }
}
