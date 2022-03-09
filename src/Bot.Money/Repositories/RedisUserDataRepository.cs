using Bot.Core.Exceptions;
using StackExchange.Redis;

namespace Bot.Money.Repositories
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
                yield return long.Parse(GetUserId(k.ToString()));
            }
        }

        public string GetClientSecret(long id)
        {
            var clientSecret = _db.StringGet(new RedisKey(id.ToString() + "_secret"));
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new NotFoundUserException();
            }
            return clientSecret;
        }

        public bool IsOwner(long id)
        {
            return long.Parse(_db.StringGet(new RedisKey("owner_id"))) == id;
        }

        public string GetUserSheet(long id)
        {
            var userSheet = _db.StringGet(new RedisKey(id.ToString() + "_sheet"));
            if (string.IsNullOrEmpty(userSheet))
            {
                throw new NotFoundUserException();
            }
            return userSheet;
        }

        private string GetUserId(string text)
        {
            var charLocation = text.IndexOf('_', StringComparison.Ordinal);
            if (charLocation > 0) return text.Substring(0, charLocation);
            return string.Empty;
        }
    }
}
