using Bot.Money.Enums;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace Bot.Money.Models
{
    public class FinanceOperationMessage
    {
        //private const string EXPENSE_PATTERN = @"^\-?\s?(\b(?![0]\b)\d{1,9}\b\.?\,?\d*)\s(.*)\s\b([1-9]|1[0-4])\b";
        //private const string INCOME_PATTERN = @"^\+{1}\s?(\b(?![0]\b)\d{1,9}\b\.?\,?\d*)\s(.*)\s\b([1-5])\b";

        // protected readonly Message _message;
        private readonly long _userId;
        private readonly ICollection<string> _messageParts;
        public FinanceOperationMessage(long userId, ICollection<string> messageParts)
        {
            _userId = userId;
            _messageParts = messageParts;
        }

        public long UserId { get { return _userId; } }

        public Expense ToExpense()
        {
            var amount = double.Parse(_messageParts.First());
            var description = _messageParts.Last();
            var category = (ExpenseCategory)Enum.Parse(typeof(ExpenseCategory), _messageParts.ElementAt(2));

            return new Expense(_userId, new DateTime().AddHours(3), amount, description, category);
        }

        public Income ToIncome()
        {
            var amount = double.Parse(_messageParts.First());
            var description = _messageParts.Last();
            var category = (IncomeCategory)Enum.Parse(typeof(IncomeCategory), _messageParts.ElementAt(2));

            return new Income(_userId, new DateTime().AddHours(3), amount, description, category);
        }

        public bool IsExpense()
        {
            return _messageParts.ElementAt(1) == "Expense";
        }

        public bool IsIncome()
        {
            return _messageParts.ElementAt(1) == "Income";
        }
    }
}
