using Bot.Core.Extension;
using Bot.Money.Repositories;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Money.Implementation
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
                yield return long.Parse(k.ToString().GetUntilOrEmpty());
            }
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
