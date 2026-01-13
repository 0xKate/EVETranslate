using EVETranslate.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EVETranslate.Services
{
    public sealed class GoogleTranslateService : ITranslationService
    {
        private readonly HttpClient _http;
        private readonly AppSettings _settings;

        public GoogleTranslateService(HttpClient http, AppSettings settings)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<string> TranslateAsync(
            string text,
            string targetLanguage,
            string? sourceLanguage = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(targetLanguage))
                throw new ArgumentException("Target language is required.", nameof(targetLanguage));

            var apiKey = _settings.GoogleTranslateApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Google Translate API key is missing. Set it in Settings.");

            // v2 endpoint (API key supported): https://translation.googleapis.com/language/translate/v2?key=...
            // :contentReference[oaicite:2]{index=2}
            var url = $"https://translation.googleapis.com/language/translate/v2?key={Uri.EscapeDataString(apiKey)}";

            var body = new TranslateRequest
            {
                Q = text,
                Target = targetLanguage,
                Source = string.IsNullOrWhiteSpace(sourceLanguage) ? null : sourceLanguage,
                Format = "text"
            };

            using var resp = await _http.PostAsJsonAsync(url, body, cancellationToken);
            resp.EnsureSuccessStatusCode();

            var data = await resp.Content.ReadFromJsonAsync<TranslateResponse>(cancellationToken: cancellationToken);
            var translated = data?.Data?.Translations is { Length: > 0 } t ? t[0].TranslatedText : null;

            return translated ?? string.Empty;
        }

        private sealed class TranslateRequest
        {
            [JsonPropertyName("q")]
            public string Q { get; set; } = "";

            [JsonPropertyName("target")]
            public string Target { get; set; } = "";

            [JsonPropertyName("source")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Source { get; set; }

            [JsonPropertyName("format")]
            public string Format { get; set; } = "text";
        }

        private sealed class TranslateResponse
        {
            [JsonPropertyName("data")]
            public TranslateData? Data { get; set; }
        }

        private sealed class TranslateData
        {
            [JsonPropertyName("translations")]
            public TranslationItem[]? Translations { get; set; }
        }

        private sealed class TranslationItem
        {
            [JsonPropertyName("translatedText")]
            public string? TranslatedText { get; set; }

            // Sometimes present if you omit source language:
            [JsonPropertyName("detectedSourceLanguage")]
            public string? DetectedSourceLanguage { get; set; }
        }
    }
}
