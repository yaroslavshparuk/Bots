﻿using Bot.Core.Abstractions;
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
            var arrayToDivide = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };
            var multiDimenArray = arrayToDivide.Split(2);
            var expectedArray = new List<List<int>>() {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 },
                new List<int> { 7, 8 },
                new List<int> { 9, 10 },};
            Assert.Equal(expectedArray, multiDimenArray);

            multiDimenArray = arrayToDivide.Split(3);
            expectedArray = new List<List<int>>() {
                new List<int> { 1, 2, 3 },
                new List<int> { 4, 5, 6},
                new List<int> { 7, 8 , 9 },
                new List<int> { 10 },};
            Assert.Equal(expectedArray, multiDimenArray);
        }

        [Fact]
        public void GetAppropriateCommandOnMessageTest()
        {
            var budgetRepository = new Mock<IBudgetRepository>();
            var operationCommandHistory = new Mock<ICommandSteps>();
            var message = new Message();
            var commands = new List<IBotCommand>()
            {
                new DownloadCommand(budgetRepository.Object),
                new FinanceOperationCommand(operationCommandHistory.Object, budgetRepository.Object),
                new HelpCommand()
            };            

            message.Text = "/help";
            message.Chat = new Chat { Id = 10 };
            var expectedHelpCommand = commands.FindExecutableCommandOnMessage(message);
            Assert.IsType<HelpCommand>(expectedHelpCommand);

            message.Text = "/download";
            var expectedDownloadCommand = commands.FindExecutableCommandOnMessage(message);
            Assert.IsType<DownloadCommand>(expectedDownloadCommand);

            message.Text = "100";
            var expectedFinanceCommand = commands.FindExecutableCommandOnMessage(message);
            Assert.IsType<FinanceOperationCommand>(expectedFinanceCommand);

            message.Text = "Income";
            operationCommandHistory.Setup(x => x.IsStarted(10)).Returns(true);
            expectedFinanceCommand = commands.FindExecutableCommandOnMessage(message);
            Assert.IsType<FinanceOperationCommand>(expectedFinanceCommand);
        }
    }
}
