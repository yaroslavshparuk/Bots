using Bot.Money.Jobs;
using Bot.Money.Repositories;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Jobs
{
    public class ResetMonthAndSendArchiveJobTest
    {
        [Fact]
        public async void InvokeTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            var userDataRepository = new Mock<IUserDataRepository>();
            var botClient = new Mock<ITelegramBotClient>();
            var sendDocumentAsyncHasBeenCalled = false;
            var sendTextAsyncHasBeenCalled = false;
            var resetMonthHasBeenCalled = false;
            var job = new ResetMonthAndSendArchiveJob(userDataRepository.Object, budgetRepository.Object, botClient.Object);

            userDataRepository.Setup(x => x.GetAllUsers()).Returns(new List<long> { 10 });
            budgetRepository.Setup(x => x.DownloadArchive(10)).ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));
            botClient.Setup(x => x.SendDocumentAsync(It.IsAny<ChatId>(), It.IsAny<InputOnlineFile>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>(), It.IsAny<InputMedia>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => sendDocumentAsyncHasBeenCalled = true);

            botClient.Setup(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => sendTextAsyncHasBeenCalled = true);

            budgetRepository.Setup(x => x.ResetMonth(10))
                     .Callback(() => resetMonthHasBeenCalled = true);

            await job.Invoke();

            Assert.True(sendDocumentAsyncHasBeenCalled);
            Assert.True(sendTextAsyncHasBeenCalled);
            Assert.True(resetMonthHasBeenCalled);
        }
    }
}
