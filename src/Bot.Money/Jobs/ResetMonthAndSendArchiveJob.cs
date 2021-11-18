using Bot.Money.Repositories;
using Coravel.Invocable;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Bot.Money.Jobs
{
    public class ResetMonthAndSendArchiveJob : IInvocable
    {
        private readonly TelegramBotClient _botClient = new(ConfigurationManager.AppSettings["money_bot_token"]);
        private readonly IUserDataRepository _userDataRepository;
        private readonly IBudgetRepository _budgetRepository;

        public ResetMonthAndSendArchiveJob(IUserDataRepository userDataRepository, IBudgetRepository budgetRepository)
        {
            _userDataRepository = userDataRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task Invoke()
        {
            foreach (var id in _userDataRepository.GetAllUsers())
            {
                await _botClient.SendTextMessageAsync(id, "Hi, I reset month at your Google sheet!\n Here is your budget from previous month",
                                                      ParseMode.Default, false, true);

                using (var stream = await _budgetRepository.DownloadArchive(id))
                    await _botClient.SendDocumentAsync(id, new InputOnlineFile(stream, $"{DateTime.Now.AddMinutes(-1).ToString("MMM yyyy")}.zip"));

                await _budgetRepository.ResetMonth(id);
            }
        }
    }
}