using Bot.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bot.Domain.Models
{
    public class Income : FinanceOperation
    {
        private IncomeCategory Category;

        public Income(long userId, DateTime date, double amount, string description, IncomeCategory category): base(userId, date, amount, description)
        {
            Category = category;
        }

        public IList<object> GetTranferObject()
        {
            return new List<object>() { Date.ToString("MM/dd/yyyy h:mm tt"), Amount.ToString(), Description.ToString(), Category.ToString() };
        }
    }
}