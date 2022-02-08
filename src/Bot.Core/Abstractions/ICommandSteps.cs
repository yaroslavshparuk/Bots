using Telegram.Bot.Types;

namespace Bot.Core.Abstractions
{
    public interface ICommandSteps
    {
        bool IsStarted(long userId);
        void StartWith(Message message);
        int Passed(long userId);
        void PassWith(Message message);
        void Finish(long userId);
        public ICollection<string> CollectionOfPassed(long userId);
    }
}
