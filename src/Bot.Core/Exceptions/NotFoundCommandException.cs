namespace Bot.Core.Exceptions
{
    public class NotFoundCommandException : Exception
    {
        public NotFoundCommandException(string message = "Command not found")
            : base(message) { }
    }
}
