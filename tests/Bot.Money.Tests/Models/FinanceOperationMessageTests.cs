﻿using Bot.Abstractions.Models;
using Bot.Money.Exceptions;
using Bot.Money.Models;
using Xunit;
using Message = Bot.Abstractions.Models.Message;

namespace Bot.Money.Tests.Models
{
    public class FinanceOperationMessageTests
    {
        private readonly string[] _values = new string[] { "123", "Витрата", "Food", "Banan" };
        private readonly IChatSessionStorage _chatSessionService;

        public FinanceOperationMessageTests()
        {
            _chatSessionService = new ChatSessionStorage();
        }

        [Fact]
        public void BuildTranferObjectTest()
        {
            var testMessage = new Message(123, "test", "asd");
            var financeOperationMessage = new FinanceOperationMessage( testMessage.ChatId, new string[] { "something wrong" });

            Assert.Throws<BuildMethodException>(() => financeOperationMessage.BuildTranferObject());

            var session = _chatSessionService.UnloadOrCreate(testMessage.ChatId);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNextState(_values[i], 0);
            }

            financeOperationMessage = new FinanceOperationMessage(testMessage.ChatId, session.UnloadValues().ToList());

            var trasferObject = financeOperationMessage.BuildTranferObject();

            Assert.Equal(123, (double)trasferObject[1]);
            Assert.Equal("Banan", trasferObject[2]);
            Assert.Equal("Food", trasferObject[3]);
        }


        [Fact]
        public void TransactionRangeTest()
        {
            var testMessage = new Message(123, "test", "asd");
            var financeOperationMessage = new FinanceOperationMessage(testMessage.ChatId, new string[] { "something wrong" });

            Assert.Throws<BuildMethodException>(() => financeOperationMessage.TransactionRange());
            var session = _chatSessionService.UnloadOrCreate(testMessage.ChatId);

            for (int i = 0; i < _values.Length; i++)
            {
                session.MoveNextState(_values[i], 0);
            }

            var values = session.UnloadValues().ToList();

            financeOperationMessage = new FinanceOperationMessage(testMessage.ChatId, values);

            Assert.Equal("Transactions!B:E", financeOperationMessage.TransactionRange());

            values[1] = "Дохід";

            financeOperationMessage = new FinanceOperationMessage(testMessage.ChatId, values);

            Assert.Equal("Transactions!G:J", financeOperationMessage.TransactionRange());
        }
    }
}
