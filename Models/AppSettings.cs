using CommunityToolkit.Mvvm.ComponentModel;

namespace EVETranslate.Models
{
    public partial class AppSettings : ObservableObject
    {
        [ObservableProperty]
        private bool onlyTranslateNewMessages = true;

        [ObservableProperty]
        private string googleTranslateApiKey = string.Empty;

        [ObservableProperty]
        private string deeplApiKey = string.Empty;

        [ObservableProperty]
        private string yandexApiKey = string.Empty;
    }
}
