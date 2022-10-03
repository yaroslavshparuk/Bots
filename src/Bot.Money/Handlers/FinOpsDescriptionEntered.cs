using Bot.Core.Abstractions;
using Bot.Money.Enums;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class FinOpsDescriptionEntered : IMoneyBotInputHandler
    {
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

            await request.Client.DeleteMessageAsync(request.Message.ChatId, request.Session.LastReplyId);
            request.Session.MoveNext(request.Message.Text, 0);
            var chatId = request.Message.ChatId;
            var finOpsMessage = new FinanceOperationMessage(chatId, request.Session.UnloadValues().ToList());
            await _budgetRepository.CreateRecord(finOpsMessage);
            await request.Client.SendTextMessageAsync(chatId: chatId, text: "Added: " + finOpsMessage.ToString(), replyMarkup: new ReplyKeyboardRemove());
        }
    }
}