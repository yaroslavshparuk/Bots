namespace Bot.Money.Exceptions
{
    public class UserChoiceException : Exception
    {
        public UserChoiceException(string message)
            : base(message) { }
    }
}
