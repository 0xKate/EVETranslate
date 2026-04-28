using CommunityToolkit.Mvvm.ComponentModel;

namespace EVETranslate.Services
{
    internal sealed class AppSettingsFileDto
    {
        public bool OnlyTranslateNewMessages { get; set; } = true;

        public string GoogleTranslateApiKeyProtected { get; set; } = string.Empty;

        public string DeeplApiKeyProtected { get; set; } = string.Empty;

        public string YandexApiKeyProtected { get; set; } = string.Empty;
    }
}
