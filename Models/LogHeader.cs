namespace EVETranslate.Models
{
    public sealed class LogHeader
    {
        public string ChannelId { get; init; } = string.Empty;
        public string ChannelName { get; init; } = string.Empty;
        public string Listener { get; init; } = string.Empty;
        public DateTime SessionStarted { get; init; }
    }
}

