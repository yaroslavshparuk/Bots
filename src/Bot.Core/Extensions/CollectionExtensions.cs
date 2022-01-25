using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Telegram.Bot.Types;

namespace Bot.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            if (size <= 0) { throw new ArgumentException(); }

            for (var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IBotCommand GetAppropriateCommandOnMessage(this IEnumerable<IBotCommand> commands, Message message)
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
