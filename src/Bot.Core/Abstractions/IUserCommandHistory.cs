using Telegram.Bot.Types;

namespace Bot.Core.Abstractions
{
    public interface IUserCommandHistory
    {
        bool HasHistory(long userId);
        void StartNewHistory(Message message);
        int HistoryLength(long userId);
        void Add(Message message);
        void Clear(long userId);
        public ICollection<string> GetHistory(long userId);
    }
}
