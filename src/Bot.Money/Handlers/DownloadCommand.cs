using Bot.Core.Abstractions;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace Bot.Money.Handlers
{
    public class DownloadCommand : IMoneyBotInputHandler
    {
        private const string NAME = "/download";
        private readonly IBudgetRepository _budgetRepository;

        public DownloadCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool IsSuitable(UserRequest request)
        {
            return request.Message.Text == NAME;
        }

        public async Task Handle(UserRequest request)
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }

            var chatId = request.Message.ChatId;
            using (var stream = await _budgetRepository.DownloadArchive(chatId))
                await request.Client.SendDocumentAsync(chatId, new InputOnlineFile(stream, DateTime.Now.AddMinutes(-1).ToString("MMMM yyyy") + ".zip"));
        }
    }
}
