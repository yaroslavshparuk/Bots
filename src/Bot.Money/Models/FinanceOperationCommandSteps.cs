using Bot.Core.Abstractions;
using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace Bot.Money.Models
{
    public class FinanceOperationCommandSteps : ICommandSteps
    {
        private readonly ConcurrentDictionary<long, ICollection<string>> _steps = new();

        public bool IsStarted(long userId)
        {
            return _steps.TryGetValue(userId, out var steps) && steps.Count() > 0;
        }

        public void Start(Message message)
        {
            _steps.TryAdd(message.Chat.Id, new List<string> { message.Text });
        }

        public int Passed(long userId)
        {
            _steps.TryGetValue(userId, out var steps);
            return steps.Count;
        }

        public void Pass(Message message)
        {
            _steps.TryGetValue(message.Chat.Id, out var steps);
            steps.Add(message.Text);
        }

        public void Finish(long userId)
        {
            _steps.TryRemove(userId, out var f);
        }

        public ICollection<string> PrintPassed(long userId)
        {
            _steps.TryGetValue(userId, out var steps);
            return steps;
        }
    }
}
