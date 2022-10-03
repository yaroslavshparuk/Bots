using Bot.Core.Abstractions;
using Bot.Money.Enums;

namespace Bot.Money.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private Dictionary<long, ChatSession> _sessions = new Dictionary<long, ChatSession>();

        public void Save(long id, ChatSession session)
        {
            _sessions.Add(id, session);
        }

        public ChatSession GetOrCreate(long id)
        {
            _sessions.TryGetValue(id, out var session);

            if (session is null || session.IsCompleted)
            {
                session = new ChatSession(new Queue<int>(Enum.GetValues(typeof(FinanceOperationState)).Cast<int>()));
            }

            _sessions.Remove(id);

            return session;
        }
    }
}
