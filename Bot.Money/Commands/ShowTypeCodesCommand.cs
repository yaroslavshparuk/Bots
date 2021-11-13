using Bot.Abstractions.Interfaces;
using Bot.Money.Enums;
using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Commands
{
    public class ShowTypeCodesCommand : ICommand
    {
        private const string EXPENSE = "/exp";
        private const string INCOME = "/inc";
        public bool CanExecute(Message message)
        {
            return message.Text == EXPENSE || message.Text == INCOME ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (message.Text == EXPENSE)
            {
                var types = $"Expense types codes: {_makeCategoryReadable(Enum.GetNames(typeof(ExpenseCategory)))}";
                await botClient.SendTextMessageAsync(message.Chat, types, ParseMode.Default, false, false, 0);
            }
            else if (message.Text == INCOME)
            {
                var types = $"Income types codes: {_makeCategoryReadable(Enum.GetNames(typeof(IncomeCategory)))}";
                await botClient.SendTextMessageAsync(message.Chat, types, ParseMode.Default, false, false, 0);
            }
        }

        private string _makeCategoryReadable(string[] input)
        {
            var strBuiler = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                strBuiler.Append($"\n{i + 1}. {input[i]}");
            }
            return strBuiler.ToString();
        }
    }
}
