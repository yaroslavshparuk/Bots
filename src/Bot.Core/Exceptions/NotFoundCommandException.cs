namespace Bot.Core.Exceptions
{
    public class NotFoundCommandException : Exception
    {
        public NotFoundCommandException(string message = "Я такої команди не розумію..")
            : base(message) { }
    }
}
