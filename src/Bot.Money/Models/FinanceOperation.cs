namespace Bot.Money.Models
{
    public class FinanceOperation
    {
        private DateTime Date;
        private double Amount;
        private string Description;
        private string Category;
        public FinanceOperation(long userId, DateTime date, double amount, string description, string category)
        {
            UserId = userId;
            Date = date;
            Amount = amount;
            Description = description;
            Category =  category;
        }

        public long UserId { get; }



        public IList<object> GetTranferObject()
        {
            return new List<object>() { Date.ToString("MM/dd/yyyy h:mm tt"), Amount.ToString(), Description, Category };
        }
    }
}
