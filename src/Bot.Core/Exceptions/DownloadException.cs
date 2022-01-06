
namespace Bot.Core.Exceptions
{
    public class DownloadException : Exception
    {
        public DownloadException(string message = "There was an error downloading, please try again later or contact @shparuk")
            : base(message) { }
    }
}
