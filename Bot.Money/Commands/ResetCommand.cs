using Bot.Money.Interfaces;
using Bot.Money.Repositories;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Money.Commands
{
    public class ResetCommand : IMoneyCommand
    {
        private const string NAME = "/reset";
        private readonly IBudgetRepository _budgetRepository;

        public ResetCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }
        public bool CanExecute(Message message)
        {
            return message.Text == NAME ? true : false;
        }

        public Task Execute(Message message, ITelegramBotClient botClient)
        {
            throw new NotImplementedException();
        }
    }
}
