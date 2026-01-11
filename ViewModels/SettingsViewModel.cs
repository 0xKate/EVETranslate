using CommunityToolkit.Mvvm.ComponentModel;
using EVETranslate.Services;

namespace EVETranslate.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        public SettingsService Settings { get; }

        public SettingsViewModel(SettingsService settings)
        {
            Settings = settings;
        }
    }
}
