using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Money.Models;
using Bot.Money.Services;
using Telegram.Bot.Types;
using Xunit;

namespace Bot.Money.Tests.Models
{
    public class FinanceOperationMessageTests
    {
        private readonly string[] _values = new string[] { "123", "Expense", "Food", "Banan" };
        private readonly IChatSessionService _chatSessionService;

        public FinanceOperationMessageTests()
        {
            _chatSessionService = new ChatSessionService();
        }

        [Fact]
        public void BuildTranferObjectTest()
        {
            var testMessage = new Message { Text = "asd", Chat = new Chat { Id = 123 } };
            var financeOperationMessage = new FinanceOperationMessage( testMessage.Chat.Id, new string[] { "something wrong" });

            Assert.Throws<BuildMethodException>(() => financeOperationMessage.BuildTranferObject());

            var session = _chatSessionService.DownloadOrCreate(testMessage.Chat.Id);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNext(_values[i]);
            }

            financeOperationMessage = new FinanceOperationMessage(testMessage.Chat.Id, session.UnloadValues().ToList());

            var trasferObject = financeOperationMessage.BuildTranferObject();

            Assert.Equal(123, (double)trasferObject[1]);
            Assert.Equal("Banan", trasferObject[2]);
            Assert.Equal("Food", trasferObject[3]);
        }


        [Fact]
        public void TransactionRangeTest()
        {
            var testMessage = new Message { Text = "asd", Chat = new Chat { Id = 10 } };
            var financeOperationMessage = new FinanceOperationMessage(testMessage.Chat.Id, new string[] { "something wrong" });

            Assert.Throws<BuildMethodException>(() => financeOperationMessage.TransactionRange());
            var session = _chatSessionService.DownloadOrCreate(testMessage.Chat.Id);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNext(_values[i]);
            }

            var values = session.UnloadValues().ToList();

            financeOperationMessage = new FinanceOperationMessage(testMessage.Chat.Id, values);

            Assert.Equal("Transactions!B:E", financeOperationMessage.TransactionRange());

            values[1] = "Income";

            financeOperationMessage = new FinanceOperationMessage(testMessage.Chat.Id, values);

            Assert.Equal("Transactions!G:J", financeOperationMessage.TransactionRange());
        }
    }
}
