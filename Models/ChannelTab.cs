using System.Collections.ObjectModel;
using System.IO;

namespace EVETranslate.Models
{
    public class ChannelTab
    {
        public string Name { get; set; } = string.Empty;

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public string LogFilePath { get; set; } = string.Empty;

        internal CancellationTokenSource? TailCts { get; set; }


    }
}
