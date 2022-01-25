using Bot.Core.Abstractions;
using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace Bot.Money.Models
{
    public class FinanceOperationCommandHistory : IUserCommandHistory
    {
        private readonly ConcurrentDictionary<long, ICollection<string>> _usersCommandHistory = new();

        public bool HasHistory(long userId)
        {
            return _usersCommandHistory.TryGetValue(userId, out var history) && history.Count() > 0;
        }

        public void StartNewHistory(Message message)
        {
            _usersCommandHistory.TryAdd(message.Chat.Id, new List<string> { message.Text });
        }

        public int HistoryLength(long userId)
        {
            _usersCommandHistory.TryGetValue(userId, out var history);
            return history.Count;
        }

        public void Add(Message message)
        {
            _usersCommandHistory.TryGetValue(message.Chat.Id, out var history);
            history.Add(message.Text);
        }

        public void Clear(long userId)
        {
            _usersCommandHistory.TryRemove(userId, out var f);
        }

        public ICollection<string> GetHistory(long userId)
        {
            _usersCommandHistory.TryGetValue(userId, out var userChatHistory);
            return userChatHistory;
        }

        public bool IsExpense(long userId)
        {
            _usersCommandHistory.TryGetValue(userId, out var history);
            return history.ElementAt(1) == "Expense";
        }

        public bool IsIncome(long userId)
        {
            _usersCommandHistory.TryGetValue(userId, out var history);
            return history.ElementAt(1) == "Income";
        }
    }
}
