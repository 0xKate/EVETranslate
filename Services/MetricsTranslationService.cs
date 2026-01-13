using System;
using System.Threading;
using System.Threading.Tasks;

namespace EVETranslate.Services
{
    public sealed class MetricsTranslationService : ITranslationService
    {
        private readonly ITranslationService _inner;
        private readonly MetricsStore _store;
        private readonly string _providerKey;

        public MetricsTranslationService(ITranslationService inner, MetricsStore store, string providerKey)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _providerKey = string.IsNullOrWhiteSpace(providerKey) ? "unknown" : providerKey.Trim();
        }

        public async Task<string> TranslateAsync(
            string text,
            string targetLanguage,
            string? sourceLanguage = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var charCount = text.Length;

            try
            {
                var result = await _inner.TranslateAsync(text, targetLanguage, sourceLanguage, cancellationToken);
                _store.AddSuccess(_providerKey, DateTimeOffset.Now, charCount);
                return result;
            }
            catch
            {
                _store.AddFailure(_providerKey, DateTimeOffset.Now);
                throw;
            }
        }
    }
}
