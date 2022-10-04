using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Bot.Money.Services;
using Moq;
using Telegram.Bot;
using Xunit;
using Message = Bot.Core.Abstractions.Message;

namespace Bot.Money.Tests.Handlers
{
    public class DescriptionEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;

        public DescriptionEnteredTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
            _budgetRepository = new Mock<IBudgetRepository>();
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsFalse()
        {
            var handler = new DescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message(123, "test", "Витрата");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForDescriptionStateSessionReturnsTrue()
        {
            var handler = new DescriptionEntered(_budgetRepository.Object);
            var textMessage =  new Message(123, "test", "Apples");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            session.MoveNext("Витрата", 0);
            session.MoveNext("Food", 0);
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputIsStringThenVerifySendTextMessageAsyncAndCreateRecordWereCalled()
        {
            var handler = new DescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message(123, "test", "Apples");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("123", 0);
            session.MoveNext("Витрата", 0);
            session.MoveNext("Food", 0);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
            _budgetRepository.Verify(x => x.CreateRecord(It.IsAny<FinanceOperationMessage>()), Times.Once());
        }
    }
}
