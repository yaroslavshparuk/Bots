namespace Bot.Money.Exceptions
{
    public class NotFoundUserException : Exception
    {
        public NotFoundUserException(string message = "User wasn't found")
            : base(message) { }
    }
}
