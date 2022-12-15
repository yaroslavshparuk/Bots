namespace Bot.Money.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message) { }
    }
}
