using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Abstractions.Interfaces
{
    public interface ICommand
    {
        public bool CanExecute(Message message);
        public Task Execute(Message message, ITelegramBotClient botClient);
    }
}
