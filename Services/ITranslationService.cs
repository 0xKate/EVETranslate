namespace EVETranslate.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(
            string text,
            string targetLanguage,
            string? sourceLanguage = null,
            CancellationToken cancellationToken = default);
    }
}
