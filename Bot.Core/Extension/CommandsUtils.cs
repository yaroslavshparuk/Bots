using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Bot.Core.Extension
{
    public static class CommandsUtils
    {
        public static ICommand GetCommandToExecute(this IEnumerable<ICommand> commands, Message message)
        {
            foreach (var command in commands)
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
