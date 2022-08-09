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
            var job = new ResetMonthAndSendArchiveJob(userDataRepository.Object, budgetRepository.Object, botClient.Object);

            userDataRepository.Setup(x => x.GetAllUsers()).Returns(new List<long> { 10 }.ToAsyncEnumerable());
            budgetRepository.Setup(x => x.DownloadArchive(10)).ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));

            await job.Invoke();
            budgetRepository.Verify(x => x.ResetMonth(10), Times.Once());
        }
    }
}
