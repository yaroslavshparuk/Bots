using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Services;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Xunit;
using Message = Bot.Core.Abstractions.Message;

namespace Bot.Money.Tests.Handlers
{
    public class AmountEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public AmountEnteredTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public void IsSuitableInputIsWaitingStateSessionReturnsFalse()
        {
            var handler = new AmountEntered();
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            session.MoveNext("", 0);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsTrue()
        {
            var handler = new AmountEntered();
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputIsStartedStateSessionReturnsTrueAsync()
        {
            _botClient.Setup(x => x.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Telegram.Bot.Types.Message()));
            var handler = new AmountEntered();
            var textMessage = new Message(123, "test", "123");
            var session = _chatSessionService.GetOrCreate(textMessage.ChatId);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
        }
    }
}
