using Bot.Repository.Interfaces;
using StackExchange.Redis;

namespace Bot.Repository.Impl
{
    public class RedisUserDataRepository : IUserDataRepository
    {
        private readonly IDatabase _db;

        public RedisUserDataRepository (IDatabase db)
        {
            _db = db;
        }

        public string GetClientSecret(string id)
        {
            return _db.StringGet(new RedisKey(id));
        }

        public string GetUserSheet(string id)
        {
            return _db.StringGet(new RedisKey(id));
        }
    }
}
