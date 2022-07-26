using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Money.Enums;
using Bot.Money.Repositories;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Handlers
{
    public class FinOpsCategoryEntered : IMoneyBotInputHandler
    {
        private readonly ReplyKeyboardMarkup _skipReply = new(new[] { new KeyboardButton[] { "Skip" } }) { ResizeKeyboard = true };
        private readonly IBudgetRepository _budgetRepository;
        public FinOpsCategoryEntered(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool IsSuitable(UserRequest request)
        {
            return request.Session.CurrentState == (int)FinanceOperationState.WaitingForCategory;
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }

            var chatId = request.Message.Chat.Id;

            var expectedCategories = await _budgetRepository.GetCategories(chatId, request.Session.LastMessageText);
            if (!expectedCategories.Contains(request.Message.Text)) { throw new UserChoiceException("You should choose correct category"); }

            request.Session.MoveNext(request.Message.Text);

            await request.Client.SendTextMessageAsync(chatId: chatId, text: "Send me a description of it (optional) ", replyMarkup: _skipReply);
        }
    }
}