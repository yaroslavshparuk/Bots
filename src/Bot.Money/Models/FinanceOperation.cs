namespace Bot.Money.Models
{
    public abstract class FinanceOperation
    {
        protected DateTime Date;
        protected double Amount;
        protected string Description;

        protected FinanceOperation(long userId, DateTime date, double amount, string description)
        {
            UserId = userId;
            Date = date;
            Amount = amount;
            Description = description;
        }

        public long UserId { get; }
    }
}
