namespace EVETranslate.Services
{
    public interface ILogTailer
    {
        Task TailAsync(string path, Action<string> onLine, bool startAtEnd, CancellationToken ct);
    }
}