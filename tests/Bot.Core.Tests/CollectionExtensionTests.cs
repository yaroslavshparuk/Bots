using Bot.Core.Abstractions;
using Bot.Core.Extensions;
using Bot.Money.Commands;
using Bot.Money.Repositories;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Bot.Core.Tests
{
    public class CollectionExtensionTests
    {
        [Fact]
        public void SplitTest()
        {
            List<int> arrayToDivide = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };
            List<List<int>> expectedArray = new List<List<int>>() {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 },
                new List<int> { 7, 8 },
                new List<int> { 9, 10 },};
            Assert.Equal(expectedArray, arrayToDivide.Split(2));

            expectedArray = new List<List<int>>() {
                new List<int> { 1, 2, 3 },
                new List<int> { 4, 5, 6},
                new List<int> { 7, 8 , 9 },
                new List<int> { 10 },};
            Assert.Equal(expectedArray, arrayToDivide.Split(3));
        }

        [Fact]
        public void GetAppropriateCommandOnMessageTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            var operationCommandHistory = new Mock<IUserCommandHistory>();
            var message = new Message();
            var commands = new List<IBotCommand>()
            {
                new DownloadCommand(budgetRepository.Object),
                new FinanceOperationCommand(operationCommandHistory.Object, budgetRepository.Object),
                new HelpCommand()
            };            

            message.Text = "/help";
            message.Chat = new Chat { Id = 10 };
            var expectedHelpCommand = commands.GetAppropriateCommandOnMessage(message);
            Assert.IsType<HelpCommand>(expectedHelpCommand);

            message.Text = "/download";
            var expectedDownloadCommand = commands.GetAppropriateCommandOnMessage(message);
            Assert.IsType<DownloadCommand>(expectedDownloadCommand);

            message.Text = "100";
            var expectedFinanceCommand = commands.GetAppropriateCommandOnMessage(message);
            Assert.IsType<FinanceOperationCommand>(expectedFinanceCommand);

            message.Text = "Income";
            operationCommandHistory.Setup(x => x.HasHistory(10)).Returns(true);
            expectedFinanceCommand = commands.GetAppropriateCommandOnMessage(message);
            Assert.IsType<FinanceOperationCommand>(expectedFinanceCommand);
        }
    }
}
