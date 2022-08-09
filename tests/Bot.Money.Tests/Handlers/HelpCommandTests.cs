using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Services;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Handlers
{
    public class HelpCommandTest
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public HelpCommandTest()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
        }
        [Fact]
        public void IsSuitableTest()
        {
            var helpCommand = new HelpCommand();
            var testMessage = new Message { Text = "", Chat = new Chat { Id = 123 } };
            var request = new UserRequest(_chatSessionService.DownloadOrCreate(testMessage.Chat.Id), testMessage, _botClient.Object);
            Assert.False(helpCommand.IsSuitable(request));

            testMessage.Text = "123asd";
            Assert.False(helpCommand.IsSuitable(request));

            testMessage.Text = "/help";
            Assert.True(helpCommand.IsSuitable(request));
        }

        [Fact]
        public async Task HandleTest()
        {
            var helpCommand = new HelpCommand();
            var testMessage = new Message { Chat = new Chat { Id = 123 }, Text = "123asd" };

            var request = new UserRequest(_chatSessionService.DownloadOrCreate(testMessage.Chat.Id), testMessage, _botClient.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => helpCommand.Handle(request));

            testMessage.Text = "/help";
            await helpCommand.Handle(request);
        }
    }
}
