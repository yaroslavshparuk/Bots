using Bot.Domain.Models;
using Bot.Repositories.Interfaces;
using Bot.Services.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services.Commands
{
    public class FinanceOperationCommand : ICommand
    {
        private readonly IBudgetRepository _budgetRepository;

        public FinanceOperationCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            var financeOperationMessage = new FinanceOperationMessage(message);
            return financeOperationMessage.IsExpense() || financeOperationMessage.IsIncome() ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            var financeOperationMessage = new FinanceOperationMessage(message);
            if (financeOperationMessage.IsExpense())
            {
                var expense = financeOperationMessage.ToExpense();
                _budgetRepository.Create(expense);
                await botClient.SendTextMessageAsync(message.Chat, "Expense was added", ParseMode.Default, null, false, false, 0);
            }

            else if (financeOperationMessage.IsIncome())
            {
                var income = financeOperationMessage.ToIncome();
                _budgetRepository.Create(income);
                await botClient.SendTextMessageAsync(message.Chat, "Income was added", ParseMode.Default, null, false, false, 0);
            }
        }
    }
}
