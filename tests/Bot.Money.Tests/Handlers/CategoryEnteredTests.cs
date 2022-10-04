using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Money.Handlers;
using Bot.Money.Repositories;
using Bot.Money.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;
using Message = Bot.Core.Abstractions.Message;

namespace Bot.Money.Tests.Handlers
{
    public class CategoryEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;
        private readonly IMemoryCache _memoryCache;

        public CategoryEnteredTests()
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
            var handler = new CategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Витрата");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForCategoryStateSessionReturnsTrue()
        {
            var handler = new CategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            session.MoveNext("Витрата", 0);
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputNotMatchExistingCategoriesThrowsUserChoiceException()
        {
            var handler = new CategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Home");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            session.MoveNext("Витрата", 0);
            await Assert.ThrowsAsync<UserChoiceException>(() => handler.Handle(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputMatchExistingCategoriesThenVerifySendTextMessageAsyncWasCalled()
        {
            _botClient.Setup(x => x.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Telegram.Bot.Types.Message()));
            _budgetRepository.Setup(x => x.GetCategories(123, "Витрата")).Returns(Task.FromResult(new string[] { "Food" }.AsEnumerable()));
            var handler = new CategoryEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Food");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            session.MoveNext("Витрата", 0);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
        }
    }
}
