using Bot.Core.Abstractions;
using Bot.Money.Enums;
using Xunit;

namespace Bot.Core.Tests.Abstractions
{
    public class ChatSessionTests
    {
        private readonly string[] _values = new string[] { "123", "Expense", "Food", "Apples" };
        [Fact]
        public void MoveNextInput4MessagesThenCurrentStateIs5()
        {
            var session = new ChatSession(new Queue<int>(Enum.GetValues(typeof(FinanceOperationState)).Cast<int>()));
            Assert.Equal((int)FinanceOperationState.Started, session.CurrentState);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNext(_values[i]);
                Assert.Equal(i + 2, session.CurrentState);
                Assert.Equal(_values[i], session.LastMessageText);
            }
        }

        [Fact]
        public void UploadValuesReturnsAllTheInputValuesViaMoveNextMethod()
        {
            var session = new ChatSession(new Queue<int>(Enum.GetValues(typeof(FinanceOperationState)).Cast<int>()));

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNext(_values[i]);
            }

            Assert.Equal(_values, session.UnloadValues());
        }
    }
}
