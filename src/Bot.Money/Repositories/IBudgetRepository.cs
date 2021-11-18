using Bot.Money.Models;
using System.IO.Compression;
using Telegram.Bot.Types;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        string CreateAndGetResult(FinanceOperationMessage message);
        Task<Stream> DownloadArchive(Message message);
    }
}
