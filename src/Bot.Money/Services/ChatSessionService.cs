using Bot.Core.Abstractions;
using Bot.Money.Enums;

namespace Bot.Money.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private Dictionary<long, ChatSession> _chatSessions;
        private readonly IEnumerable<(string, int)> _valueStates;

        public ChatSessionService()
        {
            _chatSessions = new Dictionary<long, ChatSession>();
            _valueStates = Enum.GetValues(typeof(FinanceOperationState)).Cast<int>().Select(x => (string.Empty, x));
        }

        public void Save(long id, ChatSession session)
        {
            _chatSessions.Add(id, session);
        }

        public ChatSession DownloadOrCreate(long id)
        {
            _chatSessions.TryGetValue(id, out var session);

            if (session is null || session.IsCompleted)
            {
                session = new ChatSession(new Queue<(string, int)>(_valueStates));
            }

            _chatSessions.Remove(id);

            return session;
        }
    }
}
