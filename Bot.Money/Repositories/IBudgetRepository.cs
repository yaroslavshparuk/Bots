using Bot.Money.Models;
using Telegram.Bot.Types.InputFiles;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        string CreateAndGetResult(FinanceOperationMessage message);
        Task<Stream> DownloadArchive(long userId);
    }
}
