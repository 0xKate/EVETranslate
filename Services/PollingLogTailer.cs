using System.IO;

namespace EVETranslate.Services
{
    public sealed class PollingLogTailer : ILogTailerAsync
    {
        public async Task TailAsync(
            string path,
            Func<string, Task> onLine,
            bool startAtEnd,
            CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);

            if (startAtEnd)
            {
                fs.Seek(0, SeekOrigin.End);
            }
            else
            {
                await SkipEveHeaderAsync(reader, onLine, ct);
            }

            while (!ct.IsCancellationRequested)
            {
                string? line;
                while (!ct.IsCancellationRequested && (line = await reader.ReadLineAsync()) != null)
                {
                    await onLine(line);
                }

                // no new line yet
                await Task.Delay(150, ct);
            }
        }

        private static async Task SkipEveHeaderAsync(
            StreamReader sr,
            Func<string, Task> onLine,
            CancellationToken ct)
        {
            // EVE headers end right before the first message line that starts with "[ "
            // We read and discard header lines. When we hit the first message line, we
            // *emit it* so it isn't lost.
            while (!ct.IsCancellationRequested)
            {
                if (sr.Peek() == -1)
                    return; // file empty so far

                var line = await sr.ReadLineAsync(ct);
                if (line is null)
                    return;

                if (line.StartsWith("[ "))
                {
                    await onLine(line); // don't lose the first message
                    return;
                }
            }
        }
    }
}
