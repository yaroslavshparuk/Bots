namespace Bot.Core.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message) { }
    }
}
