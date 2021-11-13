using Bot.Repositories.Interfaces;
using Bot.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Services.Commands
{
    public class ResetCommand : ICommand
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
