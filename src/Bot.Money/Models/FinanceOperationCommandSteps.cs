using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
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

        public void StartWith(Message message)
        {
            if (_steps.TryGetValue(message.Chat.Id, out var value) && value.Count > 0) throw new CommandException("Command is already started");

            _steps.TryAdd(message.Chat.Id, new List<string> { message.Text });
        }

        public int Passed(long userId)
        {
            _steps.TryGetValue(userId, out ICollection<string> steps);
            return steps is null ? 0 : steps.Count;
        }

        public void PassWith(Message message)
        {
            if (message is null) throw new ArgumentNullException();

            _steps.TryGetValue(message.Chat.Id, out var steps);

            if (steps?.Count is (0 or null)) throw new CommandException("Command isn't started");

            steps.Add(message.Text);
        }

        public void Finish(long userId)
        {
            _steps.TryGetValue(userId, out var steps);
            if (steps?.Count is (0 or null)) throw new CommandException("Command isn't started to be finished");
            _steps.TryRemove(userId, out var f);
        }

        public ICollection<string> CollectionOfPassed(long userId)
        {
            _steps.TryGetValue(userId, out var steps);
            return steps ?? new string[0];
        }
    }
}
