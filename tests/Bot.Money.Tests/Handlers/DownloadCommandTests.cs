using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Repositories;
using Bot.Money.Services;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Handlers
{
    public class DownloadCommandTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public DownloadCommandTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public void IsSuitableTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            var downloadCommand = new DownloadCommand(budgetRepository.Object);
            var testMessage = new Message { Text = "", Chat = new Chat { Id = 123 } };

            var request = new UserRequest(_chatSessionService.DownloadOrCreate(testMessage.Chat.Id), testMessage, _botClient.Object);
            Assert.False(downloadCommand.IsSuitable(request));

            testMessage.Text = "123asd";
            Assert.False(downloadCommand.IsSuitable(request));

            testMessage.Text = "/download";
            Assert.True(downloadCommand.IsSuitable(request));
        }

        [Fact]
        public async void HandleTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();

            var sendDocumentAsyncHasBeenCalled = false;
            budgetRepository.Setup(x => x.DownloadArchive(123)).ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));
            _botClient.Setup(x => x.SendDocumentAsync(It.IsAny<ChatId>(), It.IsAny<InputOnlineFile>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>(), It.IsAny<InputMedia>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => sendDocumentAsyncHasBeenCalled = true);

            var downloadCommand = new DownloadCommand(budgetRepository.Object);
            var testMessage = new Message { Chat = new Chat { Id = 123 }, Text = "123asd" };
            var request = new UserRequest(_chatSessionService.DownloadOrCreate(testMessage.Chat.Id), testMessage, _botClient.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => downloadCommand.Handle(request));

            testMessage.Text = "/download";
            await downloadCommand.Handle(request);
            Assert.True(sendDocumentAsyncHasBeenCalled);
        }
    }
}
