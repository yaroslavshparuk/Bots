using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Core.Extensions;
using Bot.Money.Enums;
using Bot.Money.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class TypeEntered : IMoneyBotInputHandler
    {
        private const int _keyBoardMarkUpRowSize = 2;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMemoryCache _memoryCache;

        public TypeEntered(IBudgetRepository budgetRepository, IMemoryCache memoryCache)
        {
            _budgetRepository = budgetRepository;
            _memoryCache = memoryCache;
        }

        public bool IsSuitable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.WaitingForType;
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }
            if (request.Message.Text is not ("Дохід" or "Витрата")) { throw new UserChoiceException("Потрібно вказати 'Дохід' або 'Витрата'"); }

            var chatId = request.Message.ChatId;
            var categories = await _budgetRepository.GetCategories(chatId, request.Message.Text);
            var replyMessage = categories
                .Select(x => new InlineKeyboardButton(x) { CallbackData = x })
                .Append(new InlineKeyboardButton("❌ Відмінити ❌") { CallbackData = "Відмінити" })
                .Split(_keyBoardMarkUpRowSize);

            _memoryCache.Set(chatId, categories);

            var reply = await request.Client.SendTextMessageAsync(chatId: chatId, text: "Виберіть категорію ⤵️", replyMarkup: new InlineKeyboardMarkup(replyMessage));
            await request.Client.DeleteMessageAsync(request.Message.ChatId ,request.Session.LastReplyId);
            request.Session.MoveNext(request.Message.Text, reply.MessageId);
        }
    }
}
