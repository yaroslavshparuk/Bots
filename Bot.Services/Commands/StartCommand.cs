using Bot.Services.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services.Commands
{
    public class StartCommand : ICommand
    {
        private const string NAME = "/start";
        private const string HELP_RESPONSE = "Choose expense or income in menu";

        public bool CanExecute(Message message)
        {
            return message.Text == NAME ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(message.Chat, HELP_RESPONSE, ParseMode.Default, null, false, false, 0);
        }
    }
}
