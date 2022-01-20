using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace Bot.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            for (var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static ICommand GetAppropriateCommandOnMessage(this IEnumerable<ICommand> commands, Message message)
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
