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
    public class TypeEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;
        private readonly IMemoryCache _memoryCache;

        public TypeEnteredTests()
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
            var handler = new TypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Expense");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForTypeStateSessionReturnsTrue()
        {
            var handler = new TypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Expense");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputNotMatchExistingTypesThrowsUserChoiceException()
        {
            var handler = new TypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Bla bla");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            await Assert.ThrowsAsync<UserChoiceException>(() => handler.Handle(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputExpenseThenVerifySendTextMessageAsyncWasCalled()
        {
            _botClient.Setup(x => x.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Telegram.Bot.Types.Message()));
            _budgetRepository.Setup(x => x.GetCategories(123, "Expense")).Returns(Task.FromResult(new string[] { "Food" }.AsEnumerable()));
            var handler = new TypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message(123, "test", "Expense");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
        }
    }
}
