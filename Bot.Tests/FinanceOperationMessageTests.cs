using Bot.Domain.Models;
using System;
using System.Linq;
using Telegram.Bot.Types;
using Xunit;

namespace Bot.Tests
{
    public class FinanceOperationMessageTests
    {
        private readonly Message[] _messagesToTest = new []
             {
                new Message { Text = "test" },
                new Message { Text = "0 some exp 1" },
                new Message { Text = "-0 some exp 1" },
                new Message { Text = "- 0 some exp 1" },
                new Message { Text = "+ 0 some inc 1" },
                new Message { Text = "+0 inc 1" },
                new Message { Text = "1.123 exp 16" },
                new Message { Text = "1,123 exp 12" },
                new Message { Text = "+1,123 inc 6" },
                new Message { Text = "+1,123 inc 0" },
                new Message { Text = "+ 1,123 inc 5" },
                new Message { Text = "+1,123 inc 5" },
                new Message { Text = "+1,123 some inc 1" },
                new Message { Text = "+ 1.123 inc 5" },
                new Message { Text = "+1.123 inc 5" },
                new Message { Text = "+1.123 some inc 2" },
                new Message { Text = "+1 inc 14" },
                new Message { Text = "+1 720 1" },
                new Message { Text = "+1. inc 1" },
                new Message { Text = "- 1,123 exp 14" },
                new Message { Text = "-1,123 exp 14" },
                new Message { Text = "1,123 exp 14" },
                new Message { Text = "1,123 some exp 1" },
                new Message { Text = "1 720 1" },
                new Message { Text = "- 1.123 exp 14" },
                new Message { Text = "-1.123 exp 14" },
                new Message { Text = "1.123 exp 14" },
                new Message { Text = "1 exp 14" },
                new Message { Text = "1. exp 1" },
                new Message { Text = "+34054,91 zp 2" },
                new Message { Text = "+34054.91 zp 2" }
             };


        [Fact]
        public void IsExpenseTest()
        {
            var actual = new []
            {
                "- 1,123 exp 14",
                "-1,123 exp 14",
                "1,123 exp 14",
                "1,123 some exp 1",
                "- 1.123 exp 14",
                "-1.123 exp 14",
                "1.123 exp 14",
                "1 exp 14",
                "1 720 1",
                "1. exp 1",
                "1,123 exp 12"
            };
            var correctMeesagesCounter = 0;
            foreach (var m in _messagesToTest)
            {
                if (new FinanceOperationMessage(m).IsExpense())
                {
                    Assert.Contains(m.Text, actual);
                    correctMeesagesCounter++;
                }
            }

            Assert.Equal(correctMeesagesCounter, actual.Length);
        }

        [Fact]
        public void IsIncomeTest()
        {
            var actual = new []
            {
                "+ 1,123 inc 5",
                "+1,123 inc 5",
                "+1,123 some inc 1",
                "+ 1.123 inc 5",
                "+1.123 inc 5",
                "+1.123 some inc 2",
                "+1 720 1",
                "+1. inc 1",
                "+34054,91 zp 2",
                "+34054.91 zp 2"
            };

            var correctMeesagesCounter = 0;
            foreach (var m in _messagesToTest)
            {
                if (new FinanceOperationMessage(m).IsIncome())
                {
                    Assert.True(actual.Any(x => x == m.Text));
                    correctMeesagesCounter++;
                }
            }

            Assert.Equal(correctMeesagesCounter, actual.Length);
        }

        [Fact]
        public void ConvertToExpenseThrownExceptionTest()
        {
            var incorrectFinanceOperationMessage = new FinanceOperationMessage(
                new Message
                {
                    Chat = new Chat { Id = 1 },
                    Date = new DateTime(2021, 1, 1, 1, 1, 1, 1),
                    Text = "7.50error exp 1"
                });

            Assert.Throws<ArgumentException>(delegate
            {
                incorrectFinanceOperationMessage.ToExpense();
            });
        }

        [Fact]
        public void ConvertToincomveThrownExceptionTest()
        {
            var incorrectFinanceOperationMessage = new FinanceOperationMessage(
                new Message
                {
                    Chat = new Chat { Id = 1 },
                    Date = new DateTime(2021, 1, 1, 1, 1, 1, 1),
                    Text = "+7.50error exp 1"
                });

            Assert.Throws<ArgumentException>(delegate
            {
                incorrectFinanceOperationMessage.ToIncome();
            });
        }
    }
}
