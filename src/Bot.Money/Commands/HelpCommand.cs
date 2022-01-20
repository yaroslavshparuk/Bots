using Bot.Money.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Commands
{
    public class HelpCommand : IMoneyCommand
    {
        private const string NAME = "/help";
        private const string HELP_RESPONSE = "It's beta version. To get any info - write @shparuk";

        public bool CanExecute(Message message)
        {
            return message.Text == NAME;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(message.Chat, HELP_RESPONSE, ParseMode.Default, false, false, 0);
        }
    }
}
