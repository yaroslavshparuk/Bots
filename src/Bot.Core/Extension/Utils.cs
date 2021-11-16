using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Bot.Core.Extension
{
    public static class Utils
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

        public static string GetUntilOrEmpty(this string text, string stopAt = "_")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);
                if (charLocation > 0) return text.Substring(0, charLocation);
            }

            return string.Empty;
        }
    }
}
