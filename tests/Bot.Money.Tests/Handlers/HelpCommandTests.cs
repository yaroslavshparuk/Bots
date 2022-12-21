using Bot.Abstractions.Models;
using Bot.Money.Handlers;
using Bot.Money.Models;
using Moq;
using Telegram.Bot;
using Xunit;
using Message = Bot.Abstractions.Models.Message;

namespace Bot.Money.Tests.Handlers
{
    public class HelpCommandTest
    {
        private readonly IChatSessionStorage _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;

        public HelpCommandTest()
        {
            _chatSessionService = new ChatSessionStorage();
            _botClient = new Mock<ITelegramBotClient>();
        }
        [Fact]
        public void IsSuitableTest()
        {
            var helpCommand = new HelpCommand();
            var testMessage = new Message(123, "test", "");
            var request = new UserRequest(_chatSessionService.UnloadOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            Assert.False(helpCommand.IsExecutable(request));

            testMessage = new Message(123, "test", "123asd");
            request = new UserRequest(_chatSessionService.UnloadOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            Assert.False(helpCommand.IsExecutable(request));

            testMessage = new Message(123, "test", "/help");
            request = new UserRequest(_chatSessionService.UnloadOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            Assert.True(helpCommand.IsExecutable(request));
        }

        [Fact]
        public async Task HandleTest()
        {
            var helpCommand = new HelpCommand();
            var testMessage = new Message(123, "test", "123asd");
            var request = new UserRequest(_chatSessionService.UnloadOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => helpCommand.Handle(request));

            testMessage = new Message(123, "test", "/help");
            request = new UserRequest(_chatSessionService.UnloadOrCreate(testMessage.ChatId), testMessage, _botClient.Object);
            await helpCommand.Handle(request);
        }
    }
}
