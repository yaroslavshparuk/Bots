﻿using Bot.Abstractions.Models;
using Bot.Money.Enums;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class DescriptionEntered : IMoneyBotInput
    {
        private readonly IBudgetRepository _budgetRepository;

        public DescriptionEntered(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool IsExecutable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.WaitingForDescription;
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsExecutable(request)) { throw new ArgumentException(); }

            await request.Client.DeleteMessageAsync(request.Message.ChatId, request.Session.LastReplyId);
            var description = request.Message.Text is "Пропустити" ? string.Empty : request.Message.Text;
            request.Session.MoveNextState(description, 0);
            var chatId = request.Message.ChatId;
            var finOpsMessage = new FinanceOperationMessage(chatId, request.Session.UnloadValues().ToList());
            await _budgetRepository.CreateRecord(finOpsMessage);
            await request.Client.SendTextMessageAsync(chatId: chatId, text: "✅ Додано: " + finOpsMessage.ToString(), replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
