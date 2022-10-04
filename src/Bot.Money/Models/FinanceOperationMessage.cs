using Bot.Core.Exceptions;
using System.Text;

namespace Bot.Money.Models
{
    public class FinanceOperationMessage
    {
        private readonly ICollection<string> _parts; // refactor
        private const string _transactionsSheetName = "Transactions";

        public FinanceOperationMessage(long userId, ICollection<string> parts)
        {
            _parts = parts;
            UserId = userId;
        }

        public long UserId { get; }

        public IList<object> BuildTranferObject()
        {
            if (_parts.Count != 4) throw new BuildMethodException("Parts count should be equal 4");

            var amount = double.Parse(_parts.First().Replace(',', '.'));
            var category = _parts.ElementAt(2);
            var description = _parts.Last();
            return new List<object>() { DateTime.UtcNow.ToString("MM/dd/yyyy h:mm tt"), amount, description, category };
        }

        public string TransactionRange()
        {
            if (_parts.Count != 4) throw new BuildMethodException("Parts count should be equal 4");
            var range = new StringBuilder(_transactionsSheetName);
            var financeType = _parts.ElementAt(1);

            if (financeType is "Витрата")
            {
                range.Append("!B:E");
            }
            else if (financeType is "Дохід")
            {
                range.Append("!G:J");
            }

            return range.ToString(); 
        }

        public override string ToString()
        {
            var result = new StringBuilder($"Сума: {_parts.ElementAt(0)}, Тип операції: {_parts.ElementAt(1)}, Категорія: {_parts.ElementAt(2)}");
            var description = _parts.ElementAt(3);
            if (!string.IsNullOrEmpty(description))
            {
                result.Append($", Опис: {description}");
            }

            return result.ToString();
        }
    }
}
