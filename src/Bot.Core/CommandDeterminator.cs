using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Telegram.Bot.Types;
using System.Collections.Generic;

namespace Bot.Core
{
    public class CommandDeterminator
    {
        private readonly IEnumerable<ICommand> _commands;

        public CommandDeterminator(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public ICommand GetAppropriateCommand(Message message)
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
