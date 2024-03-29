﻿using Bot.Abstractions.Models;
using Bot.Money.Enums;
using Bot.Money.Exceptions;
using Bot.Money.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class CategoryEntered : IMoneyBotInput
    {
        private readonly InlineKeyboardMarkup _skipReply = new InlineKeyboardButton[][]
        {
            new [] { new InlineKeyboardButton("Пропустити") { CallbackData = "Пропустити" } },
            new [] { new InlineKeyboardButton("❌ Відмінити ❌") { CallbackData = "Відмінити" } },
        };
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMemoryCache _memoryCache;

        public CategoryEntered(IBudgetRepository budgetRepository, IMemoryCache memoryCache)
        {
            _budgetRepository = budgetRepository;
            _memoryCache = memoryCache;
        }

        public bool IsExecutable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.WaitingForCategory;
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsExecutable(request)) { throw new ArgumentException(); }

            var chatId = request.Message.ChatId;

            var expectedCategories = _memoryCache.Get<IEnumerable<string>>(chatId);
            if (expectedCategories is null)
            {
                expectedCategories = await _budgetRepository.GetCategories(chatId, request.Session.LastTextMessage);
            }
            if (!expectedCategories.Contains(request.Message.Text)) { throw new UserChoiceException("Потрібно вибрати категорію зі списку"); }

            var reply = await request.Client.SendTextMessageAsync(chatId: chatId, text: "Опис ⤵️", replyMarkup: _skipReply);
            await request.Client.DeleteMessageAsync(request.Message.ChatId, request.Session.LastReplyId);
            request.Session.MoveNextState(request.Message.Text, reply.MessageId);
        }
    }
}
