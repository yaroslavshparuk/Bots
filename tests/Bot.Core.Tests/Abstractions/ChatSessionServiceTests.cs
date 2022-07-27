using Bot.Core.Abstractions;
using Bot.Money.Enums;
using Bot.Money.Services;
using Xunit;

namespace Bot.Core.Tests.Abstractions
{
    public class ChatSessionServiceTests
    {
        [Fact]
        public void UnloadNewSessionChangeItThenSaveAndUnloadToAssert()
        {
            var chatId = 12;
            var service = new ChatSessionService();
            var session = service.DownloadOrCreate(chatId);

            Assert.Equal((int)FinanceOperationState.Started, session.CurrentState);
            session.MoveNext("123");
            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);

            service.Save(chatId, session);

            session = service.DownloadOrCreate(chatId);
            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);
        }
    }
}
