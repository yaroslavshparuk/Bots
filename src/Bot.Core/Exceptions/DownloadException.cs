namespace Bot.Core.Exceptions
{
    public class DownloadException : Exception
    {
        public DownloadException(string message = "Тут немає, що завантажити, пишіть @shparuk")
            : base(message) { }
    }
}
