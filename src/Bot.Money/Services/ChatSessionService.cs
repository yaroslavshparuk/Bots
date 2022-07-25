using Bot.Core.Abstractions;
using Bot.Money.Enums;

namespace Bot.Money.Services
{
    internal class ChatSessionService : IChatSessionService
    {
        private IList<ChatSession> _sessions;

        public void Save(ChatSession session)
        {
            _sessions.Add(session);
        }

        public ChatSession Upload(long chatId)
        {
            var session = _sessions.FirstOrDefault(x => x.ChatId == chatId);

            if (session is null)
            {
                session = new ChatSession(new Queue<int>(Enum.GetValues(typeof(FinanceOperationState)).Cast<int>()));
            }
            else
            {
                _sessions.Remove(session);
            }

            return session;
        }
    }
}
