using Bot.Money.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Commands
{
    public class HelpCommand : IMoneyCommand
    {
        private const string helpResponse = "It's beta version. To get any info - write @shparuk";

        public bool CanExecute(Message message)
        {
            return message.Text is "/help";
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(message.Chat, helpResponse, ParseMode.Default, false, false, 0);
        }
    }
}
