using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Core.Abstractions
{
    public interface ICommand
    {
        public bool CanExecute(Message message);
        public Task Execute(Message message, ITelegramBotClient botClient);
    }
}
