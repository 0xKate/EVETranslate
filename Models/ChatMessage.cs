using EVETranslate.Services;

namespace EVETranslate.Models
{
    public class ChatMessage
    {
        public DateTime Timestamp { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Speaker { get; set; } = string.Empty;
        public string Listener { get; set; } = string.Empty;
        public string OriginalText { get; set; } = string.Empty;
        public string TranslatedText { get; set; } = string.Empty;
        public LangGuess.Lang GuessedLanguage { get; set; } = LangGuess.Lang.Unknown;
            public string LanguageTag => GuessedLanguage switch
        {
            LangGuess.Lang.RussianLike => "RU",
            LangGuess.Lang.ChineseLike => "CN",
            LangGuess.Lang.EnglishLike => "EN",
            _ => "?"
        };
    }
}
