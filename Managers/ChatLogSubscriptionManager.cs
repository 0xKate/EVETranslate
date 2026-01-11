using EVETranslate.Models;
using EVETranslate.Services;
using System.Runtime;
using System.Windows;

namespace EVETranslate.Parsing
{
    public sealed class ChatLogSubscriptionManager
    {

        private readonly ILogTailer _tailer;

        public ChatLogSubscriptionManager(ILogTailer tailer) => _tailer = tailer;

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

                await _tailer.TailAsync(tab.LogFilePath, line =>
                {
                    var msg = EveChatLogParser.TryParseMessageLine(line, tab.Name);
                    if (msg is null) return;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        tab.Messages.Add(msg);
                    });
                }, startAtEnd, ct);
            }, ct);
        }

        public void Stop(ChannelTab tab)
        {
            try { tab.TailCts?.Cancel(); } catch { /* ignore */ }
            tab.TailCts = null;
        }
    }
}
