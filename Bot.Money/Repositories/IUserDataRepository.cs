namespace Bot.Money.Repositories
{
    public interface IUserDataRepository
    {
        string GetClientSecret(string id);
        string GetUserSheet(string id);
    }
}
