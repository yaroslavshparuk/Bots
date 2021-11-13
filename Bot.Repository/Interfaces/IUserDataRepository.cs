namespace Bot.Repository.Interfaces
{
    public interface IUserDataRepository
    {
        string GetClientSecret(string id);
        string GetUserSheet(string id);
    }
}
