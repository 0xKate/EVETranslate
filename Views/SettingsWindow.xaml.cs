using System.Windows;
using EVETranslate.ViewModels;

namespace EVETranslate.Views
{
    public partial class SettingsWindow : Window
    {
        private bool _isInitializing;

        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = new SettingsViewModel(App.Settings);

            _isInitializing = true;
            GoogleApiKeyBox.Password = App.Settings.GoogleTranslateApiKey ?? string.Empty;
            _isInitializing = false;
        }

        private void GoogleApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
                return;

            App.Settings.GoogleTranslateApiKey = GoogleApiKeyBox.Password;
        }
    }
}
