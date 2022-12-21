using Bot.Abstractions.Models;
using Bot.Money.Handlers;
using Bot.Money.Models;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;
using Message = Bot.Abstractions.Models.Message;

namespace Bot.Money.Tests.Handlers
{
    public class AmountEnteredTests
    {
        private readonly IChatSessionStorage _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public AmountEnteredTests()
        {
            _chatSessionService = new  ChatSessionStorage();
            _botClient = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public void IsSuitableInputIsWaitingStateSessionReturnsFalse()
        {
            var handler = new AmountEntered();
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.UnloadOrCreate(textMessage.ChatId);
            session.MoveNextState("", 0);
            Assert.False(handler.IsExecutable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsTrue()
        {
            var handler = new AmountEntered();
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.UnloadOrCreate(textMessage.ChatId);
            Assert.True(handler.IsExecutable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputIsStartedStateSessionReturnsTrueAsync()
        {
            _botClient.Setup(x => x.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Telegram.Bot.Types.Message()));
            var handler = new AmountEntered();
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.UnloadOrCreate(textMessage.ChatId);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
        }
    }
}
