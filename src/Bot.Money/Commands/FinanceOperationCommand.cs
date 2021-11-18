using Bot.Money.Interfaces;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Money.Commands
{
    public class FinanceOperationCommand : IMoneyCommand
    {
        private readonly IBudgetRepository _budgetRepository;

        public FinanceOperationCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            var financeOperationMessage = new FinanceOperationMessage(message);
            return financeOperationMessage.IsExpense() || financeOperationMessage.IsIncome();
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(message.Chat, _budgetRepository.CreateAndGetResult(new FinanceOperationMessage(message)));
        }
    }
}
