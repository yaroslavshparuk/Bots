using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Repositories;
using Bot.Money.Services;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Handlers
{
    public class FinOpsAmountEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public FinOpsAmountEnteredTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public void IsSuitableInputIsWaitingStateSessionReturnsFalse()
        {
            var handler = new FinOpsAmountEntered();
            var textMessage = new Message { Text = "123", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("");
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsTrue()
        {
            var handler = new FinOpsAmountEntered();
            var textMessage = new Message { Text = "123", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputIsStartedStateSessionReturnsTrueAsync()
        {
            var handler = new FinOpsAmountEntered();
            var textMessage = new Message { Text = "123", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
            _botClient.Verify(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
