using Bot.Core.Exceptions;
using Bot.Money.Commands;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Commands
{
    public class FinanceOperationCommandTests
    {
        [Fact]
        public void CanExecuteTest()
        {
            var financeOperationCommandSteps = new FinanceOperationCommandSteps();
            var budgetRepository = new Mock<IBudgetRepository>();
            var financeOperationCommand = new FinanceOperationCommand(financeOperationCommandSteps, budgetRepository.Object);
            var testMessage = new Message { Text = "", Chat = new Chat { Id = 10 } };

            var canExecute = financeOperationCommand.CanExecute(testMessage);
            Assert.False(canExecute);

            testMessage.Text = "123asd";
            canExecute = financeOperationCommand.CanExecute(testMessage);
            Assert.False(canExecute);

            testMessage.Text = "123";
            canExecute = financeOperationCommand.CanExecute(testMessage);
            Assert.True(canExecute);

            financeOperationCommandSteps.StartWith(testMessage);
            testMessage.Text = "Income";
            canExecute = financeOperationCommand.CanExecute(testMessage);
            Assert.True(canExecute);
        }

        [Fact]
        public async void ExecuteTest()
        {
            var financeOperationCommandSteps = new FinanceOperationCommandSteps();
            var budgetRepository = new Mock<IBudgetRepository>();

            var botClient = new Mock<ITelegramBotClient>();
            var hasBeenCalled = false;
            botClient.Setup(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => hasBeenCalled = true);

            var financeOperationCommand = new FinanceOperationCommand(financeOperationCommandSteps, budgetRepository.Object);
            var testMessage = new Message { Text = "", Chat = new Chat { Id = 10 } };
            _ = Assert.ThrowsAsync<ArgumentException>(() => financeOperationCommand.Execute(testMessage, botClient.Object));
            Assert.False(hasBeenCalled);

            testMessage.Text = "100";
            await financeOperationCommand.Execute(testMessage, botClient.Object);
            Assert.True(hasBeenCalled);

            hasBeenCalled = false;
            testMessage.Text = "Whatever";
            _ = Assert.ThrowsAsync<UserChoiceException>(() => financeOperationCommand.Execute(testMessage, botClient.Object));
            Assert.False(hasBeenCalled);

            testMessage.Text = "Expense";
            var categories = new string[] { "Food", "Home", "Clothing" };
            budgetRepository.Setup(x => x.GetCategories(testMessage.Chat.Id, testMessage.Text)).Returns(Task.FromResult(categories.AsEnumerable()));
            await financeOperationCommand.Execute(testMessage, botClient.Object);
            Assert.True(hasBeenCalled);

            hasBeenCalled = false;
            testMessage.Text = "Whatever";
            _ = Assert.ThrowsAsync<UserChoiceException>(() => financeOperationCommand.Execute(testMessage, botClient.Object));
            Assert.False(hasBeenCalled);

            testMessage.Text = "Food";
            await financeOperationCommand.Execute(testMessage, botClient.Object);
            Assert.True(hasBeenCalled);

            hasBeenCalled = false;
            testMessage.Text = "Supermarket";
            await financeOperationCommand.Execute(testMessage, botClient.Object);
            Assert.True(hasBeenCalled);
        }
    }
}
