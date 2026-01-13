namespace EVETranslate.Services
{
    public interface ILogTailer
    {
        Task TailAsync(string path, Action<string> onLine, bool startAtEnd, CancellationToken ct);
    }

    public interface ILogTailerAsync
    {
        Task TailAsync(
            string path,
            Func<string, Task> onLine,
            bool startAtEnd,
            CancellationToken ct);
    }

}