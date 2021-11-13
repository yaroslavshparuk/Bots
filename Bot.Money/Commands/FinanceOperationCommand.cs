using Bot.Money.Interfaces;
using Bot.Money.Models;
using Bot.Money.Repositories;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
            return financeOperationMessage.IsExpense() || financeOperationMessage.IsIncome() ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            var result = _budgetRepository.CreateAndGetResult(new FinanceOperationMessage(message));
            await botClient.SendTextMessageAsync(message.Chat, result, ParseMode.Default, null, false, false, 0);
        }
    }
}
