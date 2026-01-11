using EVETranslate.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EVETranslate.Services
{
    public static class EveChatLogParser
    {
        private static readonly Regex ChannelIdRx = new(@"^\s*Channel ID:\s*(.+?)\s*$", RegexOptions.Compiled);

        private static readonly Regex ChannelNameRx = new(@"^\s*Channel Name:\s*(.+?)\s*$", RegexOptions.Compiled);

        private static readonly Regex ListenerRx = new(@"^\s*Listener:\s*(.+?)\s*$", RegexOptions.Compiled);

        private static readonly Regex SessionStartedRx = new(@"^\s*Session started:\s*(.+?)\s*$", RegexOptions.Compiled);

        private static readonly Regex MessageLineRx = new(@"^\s*\[\s*(?<ts>\d{4}\.\d{2}\.\d{2}\s+\d{2}:\d{2}:\d{2})\s*\]\s*(?<speaker>.*?)\s*>\s*(?<text>.*)$", RegexOptions.Compiled);

        private static readonly string[] TimestampFormat = { "yyyy.MM.dd HH:mm:ss" };


        public static bool TryParseLogHeader(string fileText, out LogHeader header)
        {
            header = new LogHeader();

            if (string.IsNullOrWhiteSpace(fileText))
                return false;

            string? channelId = null;
            string? channelName = null;
            string? listener = null;
            DateTime? sessionStarted = null;

            // Only scan the first N lines so we don't chew huge logs
            var lines = fileText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int maxLines = Math.Min(lines.Length, 80);

            for (int i = 0; i < maxLines; i++)
            {
                var line = lines[i];

                var m = ChannelIdRx.Match(line);
                if (m.Success) { channelId = m.Groups[1].Value; continue; }

                m = ChannelNameRx.Match(line);
                if (m.Success) { channelName = m.Groups[1].Value; continue; }

                m = ListenerRx.Match(line);
                if (m.Success) { listener = m.Groups[1].Value; continue; }

                m = SessionStartedRx.Match(line);
                if (m.Success)
                {
                    var raw = m.Groups[1].Value.Trim();
                    if (DateTime.TryParseExact(raw, TimestampFormat, CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeLocal, out var dt))
                    {
                        sessionStarted = dt;
                    }
                    continue;
                }

                // Optional early-exit: once we hit the first message line, stop
                if (line.Contains("]") && line.TrimStart().StartsWith("["))
                    break;
            }

            if (channelId is null || channelName is null || listener is null || sessionStarted is null)
                return false;

            header = new LogHeader
            {
                ChannelId = channelId,
                ChannelName = channelName,
                Listener = listener,
                SessionStarted = sessionStarted.Value
            };

            return true;
        }

        public static ChatMessage? TryParseMessageLine(string line, string channelName)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            // EVE logs sometimes include a UTF-8 BOM at the start of a line
            line = line.TrimStart('\uFEFF');

            // Quick reject: header/separator lines and other non-message stuff
            // (message lines always start with '[' after trimming)
            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("[")) return null;

            var m = MessageLineRx.Match(line);
            if (!m.Success) return null;

            var tsRaw = m.Groups["ts"].Value;
            if (!DateTime.TryParseExact(tsRaw, TimestampFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal, out var timestamp))
            {
                return null;
            }

            var speaker = m.Groups["speaker"].Value.Trim();
            var text = m.Groups["text"].Value.TrimEnd(); // keep internal spacing

            // Optional: ignore truly empty messages
            if (string.IsNullOrWhiteSpace(text)) return null;

            LangGuess.Lang language = LangGuess.GuessLangByScript(text);

            var translation = text; // TODO: DO translation here!
            if (language == LangGuess.Lang.EnglishLike)
            {
                translation = text;
                text = string.Empty;
            }

            return new ChatMessage
            {
                Timestamp = timestamp,
                Channel = channelName,
                Speaker = speaker.Length == 0 ? "Unknown" : speaker,
                OriginalText = text,
                TranslatedText = translation,
                GuessedLanguage = language,
            };
        }
    }

}
