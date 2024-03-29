﻿using Bot.Money.Repositories;
using Coravel.Invocable;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace Bot.Money.Jobs
{
    public class ResetMonthAndSendArchiveJob : IInvocable
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITelegramBotClient _botClient;

        public ResetMonthAndSendArchiveJob(IUserDataRepository userDataRepository, IBudgetRepository budgetRepository, ITelegramBotClient botClient)
        {
            _userDataRepository = userDataRepository;
            _budgetRepository = budgetRepository;
            _botClient = botClient;
        }

        public async Task Invoke()
        {
            await foreach (var id in _userDataRepository.GetAllUsers())
            {
                using (var fileStream = await _budgetRepository.DownloadArchive(id))
                {
                    await _budgetRepository.ResetMonth(id);
                    await _botClient.SendTextMessageAsync(id, "Hi, I reset month at your Google sheet!\nHere is your budget from previous month");
                    await _botClient.SendDocumentAsync(id, new InputOnlineFile(fileStream, $"{DateTime.Now.AddHours(-1).ToString("MMMM yyyy")}.zip"));
                }
            }
        }
    }
}