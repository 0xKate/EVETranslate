namespace EVETranslate.Services
{
    public class AppSettings
    {
        public bool OnlyTranslateNewMessages { get; set; } = true;
        public string GoogleTranslateApiKey { get; set; } = string.Empty;
    }
}
