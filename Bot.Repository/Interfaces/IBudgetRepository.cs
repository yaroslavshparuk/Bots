using Bot.Domain.Models;
using System.Collections.Generic;

namespace Bot.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        void Create(Expense expense);
        void Create(Income income);
    }
}
