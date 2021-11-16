using Bot.Money.Repositories;
using Coravel.Invocable;
using System.Configuration;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Jobs
{
    public class ReminderJob : IInvocable
    {
        private readonly TelegramBotClient _botClient = new(ConfigurationManager.AppSettings["money_bot_token"]);
        private readonly IUserDataRepository _userDataRepository;

        public ReminderJob(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        public async Task Invoke()
        {
            foreach (var id in _userDataRepository.GetAllUsers())
            {
                await _botClient.SendTextMessageAsync(id, "Hi, today is time to reset month at your budget spreadsheet",
                                                      ParseMode.Default, false, false, 0);
            }
        }
    }
}