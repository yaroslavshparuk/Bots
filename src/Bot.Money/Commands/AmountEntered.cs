using Bot.Core.Abstractions;
using Bot.Money.Enums;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Commands
{
    internal class AmountEntered : IMoneyBotCommand
    {
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, new KeyboardButton[] { _nameOfCancelButton }, }) { ResizeKeyboard = true };
        public bool CanExecute(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.NotStarted && 
                Regex.IsMatch(request.Message.Text, @"[\d]{1,9}([.,][\d]{1,6})?$");
        }

        public async Task Execute(UserRequest request)
        {
            if (!CanExecute(request)) { throw new ArgumentException(); }

            if (request.Message.Text is "Cancel")
            {
                await request.Client.SendTextMessageAsync(chatId: request.Message.Chat, text: "Canceled", replyMarkup: new ReplyKeyboardRemove());
            }

            await request.Client.SendTextMessageAsync(chatId: request.Message.Chat, text: "Is expense or income?", replyMarkup: _expOrIncReply);

            request.Session.MoveNext();
        }
    }
}
