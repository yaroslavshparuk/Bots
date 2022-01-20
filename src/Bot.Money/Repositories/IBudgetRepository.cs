using Bot.Money.Models;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        void CreateRecord(FinanceOperationMessage message);

        Task<Stream> DownloadArchive(long userId);

        Task ResetMonth(long userId);

        Task<IEnumerable<string>> GetFinanceOperationCategories(long userId, string category);
    }
}
