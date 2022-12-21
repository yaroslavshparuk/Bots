using Bot.Abstractions.Models;
using Bot.Money.Enums;
using Bot.Money.Models;
using Xunit;

namespace Bot.Abstractions.Tests.Models
{
    public class ChatSessionServiceTests
    {
        private readonly IChatSessionStorage _chatSessionService;

        public ChatSessionServiceTests()
        {
            _chatSessionService = new ChatSessionStorage();
        }

        [Fact]
        public void UnloadNewSessionChangeItThenSaveAndUnloadToAssert()
        {
            var chatId = 12;
            var session = _chatSessionService.UnloadOrCreate(chatId);

            Assert.Equal((int)FinanceOperationState.Started, session.CurrentState);
            session.MoveNextState("123", 0);
            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);

            _chatSessionService.Load(chatId, session);

            session = _chatSessionService.UnloadOrCreate(chatId);
            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);
        }
    }
}
