using Bot.Abstractions.Models;
using Bot.Money.Handlers;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Moq;
using Telegram.Bot;
using Xunit;
using Message = Bot.Abstractions.Models.Message;

namespace Bot.Money.Tests.Handlers
{
    public class DescriptionEnteredTests
    {
        private readonly IChatSessionStorage _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;

        public DescriptionEnteredTests()
        {
            _chatSessionService = new ChatSessionStorage();
            _botClient = new Mock<ITelegramBotClient>();
            _budgetRepository = new Mock<IBudgetRepository>();
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsFalse()
        {
            var handler = new DescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message(123, "test", "Витрата");
            var session = _chatSessionService.UnloadOrCreate(textMessage.ChatId);
            Assert.False(handler.IsExecutable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForDescriptionStateSessionReturnsTrue()
        {
            var handler = new DescriptionEntered(_budgetRepository.Object);
            var textMessage =  new Message(123, "test", "Apples");
            var session = _chatSessionService.UnloadOrCreate(textMessage.ChatId);
            session.MoveNextState("123", 0);
            session.MoveNextState("Витрата", 0);
            session.MoveNextState("Food", 0);
            Assert.True(handler.IsExecutable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputIsStringThenVerifySendTextMessageAsyncAndCreateRecordWereCalled()
        {
            var handler = new DescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message(123, "test", "Apples");
            var session = _chatSessionService.UnloadOrCreate(textMessage.ChatId);
            session.MoveNextState("123", 0);
            session.MoveNextState("Витрата", 0);
            session.MoveNextState("Food", 0);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
            _budgetRepository.Verify(x => x.CreateRecord(It.IsAny<FinanceOperationMessage>()), Times.Once());
        }
    }
}
