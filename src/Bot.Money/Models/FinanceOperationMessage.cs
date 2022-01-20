namespace Bot.Money.Models
{
    public class FinanceOperationMessage
    {
        private readonly long _userId;
        private readonly ICollection<string> _messageParts;
        public FinanceOperationMessage(long userId, ICollection<string> messageParts)
        {
            _userId = userId;
            _messageParts = messageParts;
        }

        public long UserId { get { return _userId; } }

        public FinanceOperation Convert()
        {
            var amount = double.Parse(_messageParts.First());
            var description = _messageParts.Last();
            var category = _messageParts.ElementAt(2);

            return new FinanceOperation(_userId, DateTime.UtcNow.AddHours(3), amount, description, category);
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
