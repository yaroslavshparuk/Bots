using Bot.Abstractions.Models;
using Bot.Money.Enums;
using Bot.Money.Models;
using Xunit;

namespace Bot.Abstractions.Tests.Models
{
    public class ChatSessionTests
    {
        private readonly string[] _values = new string[] { "123", "Витрата", "Food", "Apples" };
        private readonly IChatSessionStorage _chatSessionService;

        public ChatSessionTests()
        {
            _chatSessionService = new ChatSessionStorage();
        }

        [Fact]
        public void MoveNextInput4MessagesThenCurrentStateIs5()
        {
            var session = _chatSessionService.UnloadOrCreate(123);
            Assert.Equal((int)FinanceOperationState.Started, session.CurrentState);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNextState(_values[i], 0);
                Assert.Equal(i + 2, session.CurrentState);
                Assert.Equal(_values[i], session.LastTextMessage);
            }
        }

        [Fact]
        public void UploadValuesReturnsAllTheInputValuesViaMoveNextMethod()
        {
            var session = _chatSessionService.UnloadOrCreate(123);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNextState(_values[i], 0);
            }

            Assert.Equal(_values, session.UnloadValues());
        }
    }
}
