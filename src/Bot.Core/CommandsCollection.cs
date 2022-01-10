using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Telegram.Bot.Types;

namespace Bot.Core
{
    public class CommandsCollection
    {
        private readonly IEnumerable<ICommand> _commands;

        public CommandsCollection(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public ICommand GetAppropriateCommandOnMessage(Message message)
        {
            foreach (var command in _commands)
            {
                if (command.CanExecute(message))
                {
                    return command;
                }
            }

            throw new NotFoundCommandException();
        }
    }
}
