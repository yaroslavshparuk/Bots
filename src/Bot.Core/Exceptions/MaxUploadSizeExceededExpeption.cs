namespace Bot.Core.Exceptions
{
    public class MaxUploadSizeExceededException : Exception
    {
        public MaxUploadSizeExceededException(string message = "Ліміт розміру в 50 МБ перевищено")
            : base(message) { }
    }
}
