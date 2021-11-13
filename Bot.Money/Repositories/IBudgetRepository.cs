using Bot.Money.Models;

namespace Bot.Money.Repositories
{
    public interface IBudgetRepository
    {
        string CreateAndGetResult(FinanceOperationMessage message);
    }
}
