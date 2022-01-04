namespace Bot.Core.Exceptions
{
    public class NotFoundCommandException : Exception
    {
        public NotFoundCommandException(string message = "Appropriate command not found")
            : base(message) { }
    }
}
