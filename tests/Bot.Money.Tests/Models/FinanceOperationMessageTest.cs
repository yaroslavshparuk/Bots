using Bot.Core.Exceptions;
using Bot.Money.Commands;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Moq;
using Telegram.Bot.Types;
using Xunit;

namespace Bot.Money.Tests.Models
{
    public class FinanceOperationMessageTest
    {
        [Fact]
        public void BuildTranferObjectTest()
        {
            var steps = new FinanceOperationCommandSteps();
            var testMessage = new Message { Text = "asd", Chat = new Chat { Id = 10 } };
            var financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            Assert.Throws<BuildMethodException>(() =>  financeOperationMessage.BuildTranferObject());

            steps.StartWith(testMessage);
            for (int i = 0; i < 3; i++)
            {
                steps.PassWith(testMessage);
            }

            financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            Assert.Throws<FormatException>(() => financeOperationMessage.BuildTranferObject());
            steps.Finish(testMessage.Chat.Id);

            testMessage.Text = "10";
            steps.StartWith(testMessage);

            testMessage.Text = "Expense";
            steps.PassWith(testMessage);

            testMessage.Text = "Food";
            steps.PassWith(testMessage);

            testMessage.Text = "Banan";
            steps.PassWith(testMessage);

            financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            var trasferObject = financeOperationMessage.BuildTranferObject();

            Assert.Equal(10.0, (double)trasferObject[1]);
            Assert.Equal("Banan", trasferObject[2]);
            Assert.Equal("Food", trasferObject[3]);
        }


        [Fact]
        public void TransactionRangeTest()
        {
            var steps = new FinanceOperationCommandSteps();
            var testMessage = new Message { Text = "asd", Chat = new Chat { Id = 10 } };
            var financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            Assert.Throws<BuildMethodException>(() => financeOperationMessage.TransactionRange());

            steps.StartWith(testMessage);
            for (int i = 0; i < 3; i++)
            {
                steps.PassWith(testMessage);
            }

            financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            Assert.NotEqual("Transactions!B:E", financeOperationMessage.TransactionRange());
            steps.Finish(testMessage.Chat.Id);

            testMessage.Text = "10";
            steps.StartWith(testMessage);

            testMessage.Text = "Expense";
            steps.PassWith(testMessage);

            testMessage.Text = "Food";
            steps.PassWith(testMessage);

            testMessage.Text = "Banan";
            steps.PassWith(testMessage);

            financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            Assert.Equal("Transactions!B:E", financeOperationMessage.TransactionRange());

            steps.Finish(testMessage.Chat.Id);

            testMessage.Text = "100";
            steps.StartWith(testMessage);

            testMessage.Text = "Income";
            steps.PassWith(testMessage);

            testMessage.Text = "Gift";
            steps.PassWith(testMessage);

            testMessage.Text = "Birthday";
            steps.PassWith(testMessage);

            financeOperationMessage = new FinanceOperationMessage(
                                            testMessage.Chat.Id,
                                            steps.CollectionOfPassed(testMessage.Chat.Id));

            Assert.Equal("Transactions!G:J", financeOperationMessage.TransactionRange());
        }
    }
}
