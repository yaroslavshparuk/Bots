using Bot.Core.Abstractions;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Core.Tests.Abstractions
{
    public class DispatcherTests
    {
        private readonly Mock<IEnumerable<IBotInputHandler>> _inputHandlers;
        private readonly Mock<IChatSessionService> _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public DispatcherTests()
        {
            _inputHandlers = new Mock<IEnumerable<IBotInputHandler>>();
            _chatSessionService = new Mock<IChatSessionService>();
            _botClient = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public async void Dispatch_InputIsCancel_ReturnCancellationMessage()
        {
            var hasBeenCalled = false;

            _botClient.Setup(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => hasBeenCalled = true);

            var dispatcher = new Dispatcher(_inputHandlers.Object, _chatSessionService.Object, _botClient.Object);
            await dispatcher.Dispatch(new Message { Text = "Cancel", Chat = new Chat { Id = 123 } });

            Assert.True(hasBeenCalled);
        }
    }
}
