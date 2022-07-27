using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Services;
using Bot.Money.Enums;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;
using Bot.Core.Exceptions;

namespace Bot.Core.Tests.Abstractions
{
    public class DispatcherTests
    {
        private readonly IEnumerable<IBotInputHandler> _handlers;
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public DispatcherTests()
        {
            _handlers = new IBotInputHandler[] { new FinOpsAmountEntered() };
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public async void DispatchInputIsCancelReturnCancellationMessage()
        {
            var hasBeenCalled = false;

            _botClient.Setup(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => hasBeenCalled = true);

            var dispatcher = new Dispatcher(_handlers, _chatSessionService, _botClient.Object);
            await dispatcher.Dispatch(new Message { Text = "Cancel", Chat = new Chat { Id = 123 } });

            Assert.True(hasBeenCalled);
        }

        [Fact]
        public async void DispatchInputIsAmountReturnWaitingForTypeMessage()
        {
            var dispatcher = new Dispatcher(_handlers, _chatSessionService, _botClient.Object);
            var message = new Message { Chat = new Chat { Id = 123 }, Text = "123" };

            await dispatcher.Dispatch(message);
            var session = _chatSessionService.DownloadOrCreate(message.Chat.Id);

            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);
            Assert.Equal(message.Text, session.LastMessageText);
        }

        [Fact]
        public async void DispatchInputIsNotAmountThrowNotFoundCommandException()
        {
            var dispatcher = new Dispatcher(_handlers, _chatSessionService, _botClient.Object);
            var message = new Message { Chat = new Chat { Id = 123 }, Text = "Not amount" };
            await Assert.ThrowsAsync<NotFoundCommandException>(() => dispatcher.Dispatch(message));
        }
    }
}
