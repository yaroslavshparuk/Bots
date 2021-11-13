using Bot.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Bot.Domain.Models
{
    public class Expense : FinanceOperation
    {
        private ExpenseCategory Category;

        public Expense(long userId, DateTime date, double amount, string description, ExpenseCategory category) : base(userId, date, amount, description)
        {
            Category = category;
        }

        public IList<object> GetTranferObject()
        {
            return new List<object>() { Date.ToString("MM/dd/yyyy h:mm tt"), Amount.ToString(), Description.ToString(), Category.ToString() };
        }
    }
}
