using Bot.Abstractions.Models;
using Bot.Money.Enums;

namespace Bot.Money.Models
{
    public class ChatSessionStorage : IChatSessionStorage
    {
        private Dictionary<long, ChatSession> _chatSessions;
        private readonly IEnumerable<(string, int)> _valueStates;

        public ChatSessionStorage()
        {
            _chatSessions = new Dictionary<long, ChatSession>();
            _valueStates = Enum.GetValues(typeof(FinanceOperationState)).Cast<int>().Select(x => (string.Empty, x));
        }

        public void Load(long id, ChatSession session)
        {
            _chatSessions.Add(id, session);
        }

        public ChatSession UnloadOrCreate(long id)
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
