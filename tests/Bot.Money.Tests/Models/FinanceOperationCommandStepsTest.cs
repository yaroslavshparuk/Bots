using Bot.Core.Exceptions;
using Bot.Money.Models;
using Telegram.Bot.Types;
using Xunit;

namespace Bot.Money.Tests.Models
{
    public class FinanceOperationCommandStepsTest
    {
        [Fact]
        public void IsStartedAndStartWithTest()
        {
            var steps = new FinanceOperationCommandSteps();
            var message = new Message() { Chat = new Chat { Id = 10 } };

            Assert.False(steps.IsStarted(message.Chat.Id));

            steps.StartWith(message);
            Assert.True(steps.IsStarted(message.Chat.Id));
            Assert.Throws<CommandException>(() => steps.StartWith(message));
        }

        [Fact]
        public void PassedAndPassedWithTest()
        {
            var steps = new FinanceOperationCommandSteps();
            var message = new Message() { Chat = new Chat { Id = 10 } };
            Assert.Throws<CommandException>(() => steps.PassWith(message));

            Assert.Equal(0, steps.Passed(message.Chat.Id));
            steps.StartWith(message);

            for (var i = 2; i < 10; i++)
            {
                steps.PassWith(message);
                Assert.Equal(i, steps.Passed(message.Chat.Id));
            }
            
            Assert.Throws<ArgumentNullException>(() => steps.PassWith(null));
        }

        [Fact]
        public void FinishTest()
        {
            var steps = new FinanceOperationCommandSteps();
            var message = new Message() { Chat = new Chat { Id = 10 } };
            Assert.Throws<CommandException>(() => steps.Finish(message.Chat.Id));
            Assert.False(steps.IsStarted(message.Chat.Id));

            steps.StartWith(message);
            steps.Finish(message.Chat.Id);
            Assert.False(steps.IsStarted(message.Chat.Id));

            steps.StartWith(message);
            steps.PassWith(message);
            steps.Finish(message.Chat.Id);
            Assert.False(steps.IsStarted(message.Chat.Id));
            Assert.Throws<CommandException>(() => steps.Finish(message.Chat.Id));
        }

        [Fact]
        public void CollectionOfPassedTest()
        {
            var steps = new FinanceOperationCommandSteps();
            var message = new Message() { Chat = new Chat { Id = 10 } };

            var actual = steps.CollectionOfPassed(message.Chat.Id);
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Count);

            steps.StartWith(message);
            actual = steps.CollectionOfPassed(message.Chat.Id);
            Assert.Equal(message.Text ,actual.FirstOrDefault());

            steps.PassWith(message);
            actual = steps.CollectionOfPassed(message.Chat.Id);
            Assert.Equal(new string[] { message.Text, message.Text }, actual);

            steps.Finish(message.Chat.Id);
            actual = steps.CollectionOfPassed(message.Chat.Id);
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Count);
        }
    }
}
