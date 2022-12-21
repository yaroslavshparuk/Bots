using Bot.Abstractions.Models;
using Bot.Money.Enums;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class AmountEntered : IMoneyBotInput
    {
        private readonly InlineKeyboardMarkup _expOrIncReply = new InlineKeyboardButton[][]
        {
            new [] { new InlineKeyboardButton("Витрата") { CallbackData = "Витрата" }, new InlineKeyboardButton("Дохід") { CallbackData = "Дохід" } }, 
            new [] { new InlineKeyboardButton("❌ Відмінити ❌") { CallbackData = "Відмінити" } }
        };

        public bool IsExecutable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.Started &&
                Regex.IsMatch(request.Message.Text, @"[\d]{1,9}([.,][\d]{1,6})?$");
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsExecutable(request)) { throw new ArgumentException(); }

            var reply = await request.Client.SendTextMessageAsync(chatId: request.Message.ChatId, text: "Тип операції ⤵️", replyMarkup: _expOrIncReply);
            request.Session.MoveNextState(request.Message.Text, reply.MessageId);
        }
    }
}
