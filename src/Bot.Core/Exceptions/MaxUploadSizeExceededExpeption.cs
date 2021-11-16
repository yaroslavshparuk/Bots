using System;

namespace Bot.Core.Exceptions
{
    public class MaxUploadSizeExceededException : Exception
    {
        public MaxUploadSizeExceededException(string message = "Max upload size exceeded")
            : base(message) { }
    }
}
