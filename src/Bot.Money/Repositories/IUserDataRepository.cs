namespace Bot.Money.Repositories
{
    public interface IUserDataRepository
    {
        string GetClientSecret(long id);
        string GetUserSheet(long id);
        IEnumerable<long> GetAllUsers();
    }
}
