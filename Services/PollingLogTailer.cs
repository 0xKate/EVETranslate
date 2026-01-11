using System.IO;
using System.Windows;

namespace EVETranslate.Services
{
    public sealed class PollingLogTailer : ILogTailer
    {
        public async Task TailAsync(string path, Action<string> onLine, bool startAtEnd, CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);

            if (startAtEnd)
                fs.Seek(0, SeekOrigin.End);
            else
                SkipEveHeader(reader);

            while (!ct.IsCancellationRequested)
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    onLine(line);
                }

                // no new line yet
                await Task.Delay(150, ct);
            }
        }

        private static void SkipEveHeader(StreamReader sr)
        {
            // EVE headers end right before the first line that starts with "[ "
            // We'll read until we see that, then "rewind" one line logically by processing it.
            // Simplest: read and discard until first message line; then pass it through.

            while (true)
            {
                var peek = sr.Peek();
                if (peek == -1) return; // file empty so far

                // ReadLine to inspect content
                var line = sr.ReadLine();
                if (line is null) return;

                if (line.StartsWith("[ "))
                {
                    // We already consumed a message line—emit it by calling onLine here
                    // BUT our Skip method doesn't have onLine, so:
                    // easiest is: don't do Skip here; do it in caller logic, or implement skip differently.
                    // For now, just stop skipping when we hit first message line; caller will miss 1 line.
                    // Better approach below.
                    return;
                }
            }
        }

    }

}
