using EVETranslate.Models;

namespace EVETranslate.ViewModels
{
    public class SettingsViewModel
    {
        public AppSettings Settings { get; }

        public SettingsViewModel(AppSettings settings)
        {
            Settings = settings;
        }
    }
}
