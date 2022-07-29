namespace Bot.Money.Repositories
{
    public interface IUserDataRepository
    {
        IAsyncEnumerable<long> GetAllUsers();
        Task<string> GetClientSecret(long id);
        Task<string> GetUserSheet(long id);
    }
}
