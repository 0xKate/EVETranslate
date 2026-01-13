using EVETranslate.Models;
using System.IO;
using System.Text.Json;

namespace EVETranslate.Services
{
    public static class SettingsPersistence
    {
        private static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVETranslate");

        private static readonly string FilePath =
            Path.Combine(FolderPath, "settings.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new AppSettings();

                var json = File.ReadAllText(FilePath);
                var dto = JsonSerializer.Deserialize<AppSettingsFileDto>(json, JsonOptions) ?? new AppSettingsFileDto();

                return new AppSettings
                {
                    OnlyTranslateNewMessages = dto.OnlyTranslateNewMessages,
                    GoogleTranslateApiKey = SecretsProtector.UnprotectFromBase64(dto.GoogleTranslateApiKeyProtected)
                };
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            Directory.CreateDirectory(FolderPath);

            var dto = new AppSettingsFileDto
            {
                OnlyTranslateNewMessages = settings.OnlyTranslateNewMessages,
                GoogleTranslateApiKeyProtected = SecretsProtector.ProtectToBase64(settings.GoogleTranslateApiKey)
            };

            var json = JsonSerializer.Serialize(dto, JsonOptions);
            File.WriteAllText(FilePath, json);
        }
    }
}
