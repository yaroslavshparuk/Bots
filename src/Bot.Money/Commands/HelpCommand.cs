using Bot.Money.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Commands
{
    public class HelpCommand : IMoneyCommand
    {
        private const string NAME = "/help";
        private const string HELP_RESPONSE = 
                   "To add expense use this patterns: \n" +
                    "'100 shawarma 1'\n '-100 vegetarian shawarma 1'\n '- 100 shawarma 1'\n" +
                    "Empty or '-' - expense\n" +
                    "'100' - amount\n" +
                    "'vegetarian shawarma' - description\n" +
                    "'1' - type 'Food'\n\n" +

                    "To add income use this patterns: \n" +
                    "'+100 salary 2'\n'+ 100 some reward 3'\n" +
                    "Empty or '+' - income\n" +
                    "'100' - amount\n" +
                    "'salary' or 'some reward' - description\n" +
                    "'2' or '3' - type 'Paycheck' or 'Bonus'\n\n";

        public bool CanExecute(Message message)
        {
            return message.Text == NAME ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(message.Chat, HELP_RESPONSE, ParseMode.Default, false, false, 0);
        }
    }
}
