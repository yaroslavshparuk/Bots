using Bot.Domain.Enums;
using Bot.Domain.Models;
using System;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace Bot.Domain.Models
{
    public class FinanceOperationMessage : Message
    {
        private const string EXPENSE_PATTERN = @"^\-?\s?(\b(?![0]\b)\d{1,9}\b\.?\,?\d*)\s(.*)\s\b([1-9]|1[0-4])\b";
        private const string INCOME_PATTERN = @"^\+{1}\s?(\b(?![0]\b)\d{1,9}\b\.?\,?\d*)\s(.*)\s\b([1-5])\b";

        protected readonly Message _message;

        public FinanceOperationMessage(Message message) 
        {
            _message = message;
        }

        public Expense ToExpense()
        {
            var match = Regex.Match(_message.Text, EXPENSE_PATTERN);
            if (!match.Success || !double.TryParse(match.Groups[1].Value.Replace(',', '.'), out var amount) || !int.TryParse(match.Groups[3].Value, out var type) || amount <= 0)
            {
                throw new ArgumentException("Input is invalid");
            }

            return new Expense(_message.Chat.Id, _message.Date.AddHours(3), amount, match.Groups[2].Value, (ExpenseCategory)type);
        }

        public Income ToIncome()
        {
            var match = Regex.Match(_message.Text, INCOME_PATTERN);
            if (!match.Success || !double.TryParse(match.Groups[1].Value.Replace(',','.'), out var amount) || !int.TryParse(match.Groups[3].Value, out var type) || amount <= 0)
            {
                throw new ArgumentException("Input is invalid");
            }

            return new Income(_message.Chat.Id, _message.Date.AddHours(3), amount, match.Groups[2].Value, (IncomeCategory)type);
        }

        public bool IsExpense()
        {
            return Regex.IsMatch(_message.Text, EXPENSE_PATTERN, RegexOptions.Compiled);
        }

        public bool IsIncome()
        {
            return Regex.IsMatch(_message.Text, INCOME_PATTERN, RegexOptions.Compiled);
        }
    }
}
