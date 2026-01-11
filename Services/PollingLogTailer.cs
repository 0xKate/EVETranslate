using System.IO;

namespace EVETranslate.Services
{
    public sealed class PollingLogTailer : ILogTailer
    {
        public async Task TailAsync(string path, Action<string> onLine, CancellationToken ct)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);

            // If you want "only new messages", start at end:
            fs.Seek(0, SeekOrigin.End);

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
    }

}
