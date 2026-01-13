using EVETranslate.Models;
using EVETranslate.Services;
using EVETranslate.ViewModels;
using EVETranslate.Views;
using System.ComponentModel;
using System.Windows;

namespace EVETranslate
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Settings = SettingsPersistence.Load();
            Settings.PropertyChanged += SettingsOnPropertyChanged;

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            mainWindow.Show();
        }

        private static void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SettingsPersistence.Save(Settings);
        }
    }
}
