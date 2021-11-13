using Bot.Domain.Enums;
using Bot.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services.Commands
{
    public class ShowTypeCodesCommand : ICommand
    {
        private const string EXPENSE = "/show_expenses_types_codes";
        private const string INCOME = "/show_income_types_codes";
        public bool CanExecute(Message message)
        {
            return message.Text == EXPENSE || message.Text == INCOME ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (message.Text == "/show_expenses_types_codes")
            {
                var types = $"Expense types codes: {MakeCategoryReadable(Enum.GetNames(typeof(ExpenseCategory)))}";
                await botClient.SendTextMessageAsync(message.Chat, types, ParseMode.Default, null, false, false, 0);
            }
            else if (message.Text == "/show_income_types_codes")
            {
                var types = $"Income types codes: {MakeCategoryReadable(Enum.GetNames(typeof(IncomeCategory)))}";
                await botClient.SendTextMessageAsync(message.Chat, types, ParseMode.Default, null, false, false, 0);
            }
        }

        private string MakeCategoryReadable(string[] input)
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
