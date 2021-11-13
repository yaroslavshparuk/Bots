using System;

namespace Bot.Money.Models
{
    public abstract class FinanceOperation
    {
        protected long UserId;
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

        public string ClientSecretId
        {
            get { return UserId + "_secret"; }
        }

        public string UserSheetId
        {
            get { return UserId + "_sheet"; }
        }
    }
}
