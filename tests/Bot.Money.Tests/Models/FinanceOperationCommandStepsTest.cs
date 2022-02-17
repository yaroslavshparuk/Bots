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
    }
}
