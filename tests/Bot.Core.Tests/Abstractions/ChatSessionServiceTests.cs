using Bot.Core.Abstractions;
using Bot.Money.Enums;
using Bot.Money.Services;
using Xunit;

namespace Bot.Core.Tests.Abstractions
{
    public class ChatSessionServiceTests
    {
        private readonly IChatSessionService _chatSessionService;

        public ChatSessionServiceTests()
        {
            _chatSessionService = new ChatSessionService();
        }

        [Fact]
        public void UnloadNewSessionChangeItThenSaveAndUnloadToAssert()
        {
            var chatId = 12;
            var session = _chatSessionService.DownloadOrCreate(chatId);

            Assert.Equal((int)FinanceOperationState.Started, session.CurrentState);
            session.MoveNextState("123", 0);
            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);

            _chatSessionService.Save(chatId, session);

            session = _chatSessionService.DownloadOrCreate(chatId);
            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);
        }
    }
}
