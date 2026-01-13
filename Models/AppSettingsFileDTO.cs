namespace EVETranslate.Services
{
    internal sealed class AppSettingsFileDto
    {
        public bool OnlyTranslateNewMessages { get; set; } = true;

        // Encrypted Base64 blob on disk
        public string GoogleTranslateApiKeyProtected { get; set; } = string.Empty;
    }
}
