using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Core.Extensions;
using Bot.Money.Enums;
using Bot.Money.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
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
            if (request.Message.Text is not ("Income" or "Expense")) { throw new UserChoiceException("You should choose 'Expense' or 'Income'"); }

            var chatId = request.Message.ChatId;
            var categories = await _budgetRepository.GetCategories(chatId, request.Message.Text);
            var replyMessage = categories
                .Select(x => new InlineKeyboardButton(x) { CallbackData = x })
                .Append(new InlineKeyboardButton("Cancel") { CallbackData = "Cancel" })
                .Split(_keyBoardMarkUpRowSize);

            _memoryCache.Set(chatId, categories);

            var reply = await request.Client.SendTextMessageAsync(chatId: chatId, text: "What category is it?", replyMarkup: new InlineKeyboardMarkup(replyMessage));
            await request.Client.DeleteMessageAsync(request.Message.ChatId ,request.Session.LastReplyId);
            request.Session.MoveNext(request.Message.Text, reply.MessageId);
        }
    }
}
