using Bot.Core.Exceptions;
using Bot.Money.Exceptions;
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

        public async IAsyncEnumerable<long> GetAllUsers()
        {
            await foreach (var k in _server.KeysAsync(pattern: "*_sheet"))
            {
                yield return long.Parse(GetUserId(k.ToString()));
            }
        }

        public async Task<string> GetClientSecret(long id)
        {
            var clientSecret = await _db.StringGetAsync(new RedisKey(id.ToString() + "_secret"));
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new NotFoundUserException();
            }
            return clientSecret;
        }

        public async Task<string> GetUserSheet(long id)
        {
            var userSheet = await _db.StringGetAsync(new RedisKey(id.ToString() + "_sheet"));
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
