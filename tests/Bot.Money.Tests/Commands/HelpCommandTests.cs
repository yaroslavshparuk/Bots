using Bot.Core.Exceptions;
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
    public class HelpCommandTests
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
        public async void ExecuteTest()
        {
            var botClient = new Mock<ITelegramBotClient>();
            botClient.Setup(x => x.SendDocumentAsync(It.IsAny<ChatId>(), It.IsAny<InputOnlineFile>(), It.IsAny<string>(), It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>(), It.IsAny<InputMedia>()))
                     .Returns(Task.FromResult(new Message()));

            var helpCommand = new HelpCommand();
            var testMessage = new Message { Chat = new Chat { Id = 123 }, Text = "123asd" };
            _ = Assert.ThrowsAsync<NotFoundCommandException>(() => helpCommand.Execute(testMessage, botClient.Object));

            testMessage.Text = "/help";
            await helpCommand.Execute(testMessage, botClient.Object);
        }
    }
}
