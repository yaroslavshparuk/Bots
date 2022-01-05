using Bot.Money.Enums;
using System;
using System.Collections.Generic;

namespace Bot.Money.Models
{
    public class Income : FinanceOperation
    {
        private IncomeCategory _category;

        public Income(long userId, DateTime date, double amount, string description, IncomeCategory category) : base(userId, date, amount, description)
        {
            _category = category;
        }

        public IList<object> GetTranferObject()
        {
            return new List<object>() { Date.ToString("MM/dd/yyyy h:mm tt"), Amount.ToString(), Description.ToString(), _category.ToString() };
        }
    }
}
