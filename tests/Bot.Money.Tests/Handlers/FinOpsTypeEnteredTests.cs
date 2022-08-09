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
    public class FinOpsTypeEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;
        private readonly IMemoryCache _memoryCache;

        public FinOpsTypeEnteredTests()
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
            var handler = new FinOpsTypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Expense", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForTypeStateSessionReturnsTrue()
        {
            var handler = new FinOpsTypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Expense", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputNotMatchExistingTypesThrowsUserChoiceException()
        {
            var handler = new FinOpsTypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Bla bla", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            await Assert.ThrowsAsync<UserChoiceException>(() => handler.Handle(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputExpenseThenVerifySendTextMessageAsyncWasCalled()
        {
            _budgetRepository.Setup(x => x.GetCategories(123, "Expense")).Returns(Task.FromResult(new string[] { "Food" }.AsEnumerable()));
            var handler = new FinOpsTypeEntered(_budgetRepository.Object, _memoryCache);
            var textMessage = new Message { Text = "Expense", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
        }
    }
}
