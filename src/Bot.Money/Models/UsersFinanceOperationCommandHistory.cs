using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace Bot.Money.Models
{
    public class UsersFinanceOperationCommandHistory
    {
        private readonly ConcurrentDictionary<long, ICollection<string>> _historyOfUserChats = new();

        public bool HasUnfinishedCommand(long userId)
        {
            return _historyOfUserChats.TryGetValue(userId, out var history) && history.Count() > 0;
        }
         
        public void StartNewHistory(Message message)
        {
            _historyOfUserChats.TryAdd(message.Chat.Id, new List<string> { message.Text });
        }

        public int TakeUserHistoryLength(long userId)
        {
            _historyOfUserChats.TryGetValue(userId, out var history);
            return history.Count;
        }

        public void Push(Message message)
        {
            _historyOfUserChats.TryGetValue(message.Chat.Id, out var history);
            history.Add(message.Text);
        }

        public bool TryFlush(long userId)
        {
            return _historyOfUserChats.TryRemove(userId, out var f);
        }

        public FinanceOperationMessage Complete(long userId)
        {
            _historyOfUserChats.TryGetValue(userId, out var userChatHistory);
            TryFlush(userId);
            return new FinanceOperationMessage(userId, userChatHistory);
        }
    }
}
