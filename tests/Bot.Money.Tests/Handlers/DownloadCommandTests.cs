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
using Message = Bot.Core.Abstractions.Message;

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
            var testMessage = new Message(123, "test", "");
            var request = new UserRequest(_chatSessionService.GetOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            Assert.False(downloadCommand.IsSuitable(request));

            testMessage = new Message(123, "test", "123asd");
            request = new UserRequest(_chatSessionService.GetOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            Assert.False(downloadCommand.IsSuitable(request));

            testMessage = new Message(123, "test", "/download");
            request = new UserRequest(_chatSessionService.GetOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            Assert.True(downloadCommand.IsSuitable(request));
        }

        [Fact]
        public async void HandleTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            budgetRepository.Setup(x => x.DownloadArchive(123)).ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));

            var downloadCommand = new DownloadCommand(budgetRepository.Object);
            var testMessage = new Message(123, "test", "123asd");
            var request = new UserRequest(_chatSessionService.GetOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => downloadCommand.Handle(request));

            testMessage = new Message(123, "test", "/download");
            request = new UserRequest(_chatSessionService.GetOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            await downloadCommand.Handle(request);
        }
    }
}
