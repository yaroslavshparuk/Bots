using Bot.Money.Models;
using System.IO.Compression;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        string CreateAndGetResult(FinanceOperationMessage message);
        Task<Stream> DownloadArchive(long userId);
    }
}
