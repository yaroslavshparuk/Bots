using Bot.Money.Interfaces;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

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

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            using (var stream = await _budgetRepository.DownloadArchive(message.Chat.Id))
            {
                stream.Position = 0;
                await botClient.SendDocumentAsync(message.Chat, new InputOnlineFile(stream, "test.pdf"));
            }
        }
    }
}
