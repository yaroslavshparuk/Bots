using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Money.Handlers;
using Bot.Money.Repositories;
using Bot.Money.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Handlers
{
    public class FinOpsCategoryEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;
        private readonly IMemoryCache _memoryCache;

        public FinOpsCategoryEnteredTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
            _budgetRepository = new Mock<IBudgetRepository>();
            var services = new ServiceCollection();
            services.AddMemoryCache();
            _memoryCache = services.BuildServiceProvider().GetService<IMemoryCache>();
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsFalse()
        {
            var handler = new FinOpsCategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Expense", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForCategoryStateSessionReturnsTrue()
        {
            var handler = new FinOpsCategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "123", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            session.MoveNext("Expense");
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputNotMatchExistingCategoriesThrowsUserChoiceException()
        {
            var handler = new FinOpsCategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Home", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            session.MoveNext("Expense");
            await Assert.ThrowsAsync<UserChoiceException>(() => handler.Handle(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputMatchExistingCategoriesThenVerifySendTextMessageAsyncWasCalled()
        {
            _budgetRepository.Setup(x => x.GetCategories(123, "Expense")).Returns(Task.FromResult(new string[] { "Food" }.AsEnumerable()));
            var handler = new FinOpsCategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Food", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            session.MoveNext("Expense");
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
            _botClient.Verify(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
