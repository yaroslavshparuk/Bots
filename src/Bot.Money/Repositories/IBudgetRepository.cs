using Bot.Money.Models;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        void CreateRecord(FinanceOperationMessage financeOperationMessage);

        Task<Stream> DownloadArchive(long userId);

        Task ResetMonth(long userId);

        Task<IEnumerable<string>> GetCategories(long userId, string category);
    }
}
