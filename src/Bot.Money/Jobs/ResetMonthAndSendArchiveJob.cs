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
                using (var stream = await _budgetRepository.DownloadArchive(id))
                    await _botClient.SendDocumentAsync(id, new InputOnlineFile(stream, $"{DateTime.Now.AddHours(-1).ToString("MMMM yyyy")}.zip"), 
                                                       null,ParseMode.Default, true);
                await _budgetRepository.ResetMonth(id);
                await _botClient.SendTextMessageAsync(id, "Hi, I reset month at your Google sheet!\nHere is your budget from previous month",
                                                      ParseMode.Default, false, true);
            }
        }
    }
}