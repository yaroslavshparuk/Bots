namespace Bot.Core.Exceptions
{
    public class UserChoiceException : Exception
    {
        public UserChoiceException(string message)
            : base(message) { }
    }
}
