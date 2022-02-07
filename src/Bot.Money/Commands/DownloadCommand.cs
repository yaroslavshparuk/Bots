using Bot.Core.Exceptions;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Bot.Money.Commands
{
    public class DownloadCommand : IMoneyBotCommand
    {
        private const string NAME = "/download";
        private readonly IBudgetRepository _budgetRepository;

        public DownloadCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            return message.Text == NAME;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (!CanExecute(message)) { throw new ArgumentException(); }

            using (var stream = await _budgetRepository.DownloadArchive(message.Chat.Id))
                await botClient.SendDocumentAsync(message.Chat, new InputOnlineFile(stream, DateTime.Now.AddMinutes(-1).ToString("MMMM yyyy") + ".zip"));
        }
    }
}
