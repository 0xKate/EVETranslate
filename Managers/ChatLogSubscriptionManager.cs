using EVETranslate.Models;
using EVETranslate.Services;
using System.Runtime;
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

            if (text == null || text == string.Empty)
                return string.Empty;

            return msg.GuessedLanguage switch
            {
                LangGuess.Lang.EnglishLike => string.Empty,

                LangGuess.Lang.ChineseLike =>
                    await App.Translator.TranslateAsync(text, "en", "zh", ct),

                LangGuess.Lang.RussianLike =>
                    await App.Translator.TranslateAsync(text, "en", "ru", ct),

                _ => text
            };
        }
    }
}
