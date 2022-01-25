namespace Bot.Money.Models
{
    public class FinanceOperationMessage
    {
        private readonly ICollection<string> _messageParts;

        public FinanceOperationMessage(long userId, ICollection<string> messageParts)
        {
            _messageParts = messageParts;
            UserId = userId;
        }

        public long UserId { get; }

        public IList<object> GetTranferObject()
        {
            var amount = double.Parse(_messageParts.First().Replace(',', '.'));
            var description = _messageParts.Last();
            var category = _messageParts.ElementAt(2);

            return new List<object>() { DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt"), amount, description, category };
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
