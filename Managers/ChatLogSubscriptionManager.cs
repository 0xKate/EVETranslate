using EVETranslate.Models;
using EVETranslate.Services;
using System.Net.Http;
using System.Windows;

namespace EVETranslate.Parsing
{
    public sealed class ChatLogSubscriptionManager
    {
        private readonly ILogTailerAsync _tailer;

        public ChatLogSubscriptionManager(ILogTailerAsync tailer) => _tailer = tailer;

        public void Start(ChannelTab tab)
        {
            Stop(tab);

            if (string.IsNullOrWhiteSpace(tab.LogFilePath))
                return;

            tab.TailCts = new CancellationTokenSource();
            var ct = tab.TailCts.Token;

            _ = Task.Run(async () =>
            {
                bool startAtEnd = App.Settings.OnlyTranslateNewMessages;

                await _tailer.TailAsync(
                    tab.LogFilePath,
                    async line =>
                    {
                        var msg = EveChatLogParser.TryParseMessageLine(line, tab.Name);
                        if (msg is null) return;

                        // Do async work OFF the UI thread
                        msg.TranslatedText = await TranslateIfNeededAsync(msg, ct);

                        if (msg.TranslatedText == string.Empty)
                        {
                            msg.TranslatedText = msg.OriginalText;
                            msg.OriginalText = string.Empty;
                        }

                        // Then update UI
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            tab.Messages.Add(msg);
                        });
                    },
                    startAtEnd,
                    ct);
            }, ct);
        }

        public void Stop(ChannelTab tab)
        {
            try { tab.TailCts?.Cancel(); } catch { /* ignore */ }
            tab.TailCts = null;
        }

        private static async Task<string> TranslateIfNeededAsync(ChatMessage msg, CancellationToken ct)
        {
            var text = msg.OriginalText;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            try
            {
                var result = msg.GuessedLanguage switch
                {
                    LangGuess.Lang.EnglishLike => string.Empty,

                    LangGuess.Lang.ChineseLike =>
                        await App.Translator.TranslateAsync(text, "en", "zh", ct),

                    LangGuess.Lang.RussianLike =>
                        await App.Translator.TranslateAsync(text, "en", "ru", ct),

                    _ => text
                };

                // Translation succeeded — clear the error flag so future failures show again
                Interlocked.Exchange(ref _translationErrorShown, 0);
                return result;
            }
            catch (OperationCanceledException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                await ShowTranslationErrorAsync(ex);
                return string.Empty;
            }
        }


        private static int _translationErrorShown = 0; // 0 = false, 1 = true

        private static async Task ShowTranslationErrorAsync(Exception ex)
        {
            if (Interlocked.CompareExchange(ref _translationErrorShown, 1, 0) != 0) return;

            // Do NOT reset the flag here — it stays set until a successful translation clears it
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var message = ex switch
                {
                    HttpRequestException { StatusCode: System.Net.HttpStatusCode.Forbidden } =>
                        "Translation failed: API key is invalid or unauthorized (403).\n\nPlease check your API key in Settings.",

                    HttpRequestException { StatusCode: System.Net.HttpStatusCode.TooManyRequests } =>
                        "Translation failed: API rate limit exceeded (429).\n\nPlease wait a moment before retrying.",

                    HttpRequestException { StatusCode: System.Net.HttpStatusCode.ServiceUnavailable } =>
                        "Translation failed: Translation service is temporarily unavailable (503).\n\nPlease try again later.",

                    HttpRequestException httpEx =>
                        $"Translation failed: Network error.\n\nDetails: {httpEx.Message}",

                    _ =>
                        $"Translation failed: Unexpected error.\n\nDetails: {ex.Message}"
                };

                MessageBox.Show(
                    message,
                    "Translation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            });
        }
    }
}