using Bot.Money.Models;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        string CreateAndGetResult(FinanceOperationMessage message);
        Task<Stream> DownloadArchive(long userId);
        Task ResetMonth(long userId);
    }
}
