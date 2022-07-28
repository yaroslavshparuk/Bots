using Bot.Core.Abstractions;
using Bot.Money.Enums;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class FinOpsDescriptionEntered : IMoneyBotInputHandler
    {
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, new KeyboardButton[] { "Cancel" }, }) { ResizeKeyboard = true };
        private readonly IBudgetRepository _budgetRepository;

        public FinOpsDescriptionEntered(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool IsSuitable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.WaitingForDescription;
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }

            request.Session.MoveNext(request.Message.Text);
            var chatId = request.Message.Chat.Id;
            _budgetRepository.CreateRecord(new FinanceOperationMessage(chatId, request.Session.UnloadValues().ToList()));
            await request.Client.SendTextMessageAsync(chatId: chatId, text: "Added", replyMarkup: new ReplyKeyboardRemove());
        }
    }
}