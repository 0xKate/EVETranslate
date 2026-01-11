using CommunityToolkit.Mvvm.ComponentModel;

namespace EVETranslate.Services
{
    public class SettingsService : ObservableObject
    {
        private readonly AppSettings _settings = new();

        public bool OnlyTranslateNewMessages
        {
            get => _settings.OnlyTranslateNewMessages;
            set => SetProperty(_settings.OnlyTranslateNewMessages, value, _settings, (s, v) => s.OnlyTranslateNewMessages = v);
        }

        public string GoogleTranslateApiKey
        {
            get => _settings.GoogleTranslateApiKey;
            set => SetProperty(_settings.GoogleTranslateApiKey, value, _settings, (s, v) => s.GoogleTranslateApiKey = v);
        }
    }
}
