using Telegram.Bot.Types;

namespace Bot.Core.Abstractions
{
    public class ChatSession
    {
        private readonly Queue<int> _states;

        public ChatSession(Queue<int> states)
        {
            _states = states;
            CurrentState = _states.Peek();
        }

        public int CurrentState { get; private set; }

        public long ChatId { get; private set; }

        public void MoveNext()
        {
            CurrentState = _states.Dequeue();
        }
    }
}
