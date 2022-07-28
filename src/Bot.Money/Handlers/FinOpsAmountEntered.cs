using Bot.Core.Abstractions;
using Bot.Money.Enums;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class FinOpsAmountEntered : IMoneyBotInputHandler
    {
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, new KeyboardButton[] { "Cancel" }, }) { ResizeKeyboard = true };

        public bool IsSuitable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.Started && 
                Regex.IsMatch(request.Message.Text, @"[\d]{1,9}([.,][\d]{1,6})?$");
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }

            request.Session.MoveNext(request.Message.Text);
            await request.Client.SendTextMessageAsync(chatId: request.Message.Chat, text: "Is expense or income?", replyMarkup: _expOrIncReply);
        }
    }
}
