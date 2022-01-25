using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Core.Abstractions
{
    public interface IBotCommand
    {
        public bool CanExecute(Message message);
        public Task Execute(Message message, ITelegramBotClient botClient);
    }
}
