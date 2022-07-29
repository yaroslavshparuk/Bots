using Bot.Core.Abstractions;
using Bot.Money.Enums;
using Bot.Money.Services;
using Xunit;

namespace Bot.Core.Tests.Abstractions
{
    public class ChatSessionTests
    {
        private readonly string[] _values = new string[] { "123", "Expense", "Food", "Apples" };
        private readonly IChatSessionService _chatSessionService;

        public ChatSessionTests()
        {
            _chatSessionService = new ChatSessionService();
        }

        [Fact]
        public void MoveNextInput4MessagesThenCurrentStateIs5()
        {
            var session = _chatSessionService.DownloadOrCreate(123);
            Assert.Equal((int)FinanceOperationState.Started, session.CurrentState);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNext(_values[i]);
                Assert.Equal(i + 2, session.CurrentState);
                Assert.Equal(_values[i], session.LastTextMessage);
            }
        }

        [Fact]
        public void UploadValuesReturnsAllTheInputValuesViaMoveNextMethod()
        {
            var session = _chatSessionService.DownloadOrCreate(123);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNext(_values[i]);
            }

            Assert.Equal(_values, session.UnloadValues());
        }
    }
}
