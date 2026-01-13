using EVETranslate.Models;
using EVETranslate.Services;
using EVETranslate.ViewModels;
using EVETranslate.Views;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;

namespace EVETranslate
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; } = new();
        public static HttpClient Http { get; } = new HttpClient();
        public static MetricsStore Metrics { get; private set; } = null!;
        public static ITranslationService Translator { get; private set; } = null!;

        // Optional: keep both around if you want fast switching
        private static ITranslationService _googleTracked = null!;
        //private static ITranslationService _deeplTracked = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Settings = SettingsPersistence.Load();
            Settings.PropertyChanged += SettingsOnPropertyChanged;

            Metrics = new MetricsStore();

            // Build providers
            var google = new GoogleTranslateService(Http, Settings);
            _googleTracked = new MetricsTranslationService(google, Metrics, "google");

            Translator = _googleTracked; // Default

            //var deepl = new DeeplTranslateService(Http, Settings);
            //_deeplTracked = new MetricsTranslationService(deepl, Metrics, "deepl");

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
