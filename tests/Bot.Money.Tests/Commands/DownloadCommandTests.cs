using Bot.Core.Exceptions;
using Bot.Money.Commands;
using Bot.Money.Repositories;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Commands
{
    public class DownloadCommandTests
    {
        [Fact]
        public void CanExecuteTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            var downloadCommand = new DownloadCommand(budgetRepository.Object);
            var testMessage = new Message { Text = "" };
            var canExecute = downloadCommand.CanExecute(testMessage);
            Assert.False(canExecute);

            testMessage.Text = "123asd";
            canExecute = downloadCommand.CanExecute(testMessage);
            Assert.False(canExecute);

            testMessage.Text = "/download";
            canExecute = downloadCommand.CanExecute(testMessage);
            Assert.True(canExecute);
        }

        [Fact]
        public async void ExecuteTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            var botClient = new Mock<ITelegramBotClient>();

            budgetRepository.Setup(x => x.DownloadArchive(123)).ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));
            botClient.Setup(x => x.SendDocumentAsync(It.IsAny<ChatId>(), It.IsAny<InputOnlineFile>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>(), It.IsAny<InputMedia>()))
                     .Returns(Task.FromResult(new Message()));

            var downloadCommand = new DownloadCommand(budgetRepository.Object);
            var testMessage = new Message { Chat = new Chat { Id = 123 }, Text = "123asd" };
            _ = Assert.ThrowsAsync<NotFoundCommandException>(() => downloadCommand.Execute(testMessage, botClient.Object));

            testMessage.Text = "/download";
            await downloadCommand.Execute(testMessage, botClient.Object);
        }
    }
}
