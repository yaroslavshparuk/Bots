using Bot.Money.Repositories;
using StackExchange.Redis;

namespace Bot.Money.Impl
{
    public class RedisUserDataRepository : IUserDataRepository
    {
        private readonly IDatabase _db;
        private readonly IServer _server;
        public RedisUserDataRepository(ConnectionMultiplexer multiplexer)
        {
            _db = multiplexer.GetDatabase();
            _server = multiplexer.GetServer(multiplexer.GetEndPoints().FirstOrDefault());
        }

        public IEnumerable<long> GetAllUsers()
        {
            foreach (var k in _server.Keys(pattern: "*_sheet"))
            {
                yield return long.Parse(TakeUserId(k.ToString()));
            }
        }

        public string GetClientSecret(long id)
        {
            return _db.StringGet(new RedisKey(id.ToString() + "_secret"));
        }

        public string GetUserSheet(long id)
        {
            return _db.StringGet(new RedisKey(id.ToString() + "_sheet"));
        }

        private string TakeUserId(string text)
        {
            var charLocation = text.IndexOf('_', StringComparison.Ordinal);
            if (charLocation > 0) return text.Substring(0, charLocation);
            return string.Empty;
        }
    }
}
