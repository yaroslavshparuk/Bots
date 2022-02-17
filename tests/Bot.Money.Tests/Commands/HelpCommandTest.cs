using Bot.Money.Commands;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Commands
{
    public class HelpCommandTest
    {
        [Fact]
        public void CanExecuteTest()
        {
            var helpCommand = new HelpCommand();
            var testMessage = new Message { Text = "" };

            var canExecute = helpCommand.CanExecute(testMessage);
            Assert.False(canExecute);

            testMessage.Text = "123asd";
            canExecute = helpCommand.CanExecute(testMessage);
            Assert.False(canExecute);

            testMessage.Text = "/help";
            canExecute = helpCommand.CanExecute(testMessage);
            Assert.True(canExecute);
        }

        [Fact]
        public async Task ExecuteTest()
        {
            var botClient = new Mock<ITelegramBotClient>();
            var hasBeenCalled = false;
            botClient.Setup(x => x.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(new Message()))
                     .Callback(() => hasBeenCalled = true);

            var helpCommand = new HelpCommand();
            var testMessage = new Message { Chat = new Chat { Id = 123 }, Text = "123asd" };
            _ = Assert.ThrowsAsync<ArgumentException>(() => helpCommand.Execute(testMessage, botClient.Object));

            testMessage.Text = "/help";
            await helpCommand.Execute(testMessage, botClient.Object);
            Assert.True(hasBeenCalled);
        }
    }
}
