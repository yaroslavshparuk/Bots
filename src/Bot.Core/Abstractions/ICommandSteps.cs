using Telegram.Bot.Types;

namespace Bot.Core.Abstractions
{
    public interface ICommandSteps
    {
        bool IsStarted(long userId);
        void Start(Message message);
        int Passed(long userId);
        void Pass(Message message);
        void Finish(long userId);
        public ICollection<string> CollectionOfPassed(long userId);
    }
}
