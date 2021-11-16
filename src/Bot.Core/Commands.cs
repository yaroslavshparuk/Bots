using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Telegram.Bot.Types;

namespace Bot.Core
{
    public class Commands 
    {
        private readonly IEnumerable<ICommand> _commands;

        public Commands(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public ICommand DetermineAndGetCommand(Message message)
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
